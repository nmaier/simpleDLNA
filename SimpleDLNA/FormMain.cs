using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections.Concurrent;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormMain : Form, IAppender, IDisposable
  {
    private struct LogEntry
    {
      public string Level;
      public string Message;
      public string Class;
      public string Exception;
    }
    private bool logging = false;
    private static readonly Properties.Settings Config = Properties.Settings.Default;
    private HttpServer httpServer;
    private readonly FileInfo cacheFile = new FileInfo(Path.Combine(cacheDir, "sdlna.cache"));
#if DEBUG
    private readonly FileInfo logFile = new FileInfo(Path.Combine(cacheDir, "sdlna.dbg.log"));
#else
    private readonly FileInfo logFile = new FileInfo(Path.Combine(cacheDir, "sdlna.log"));
#endif
    private bool canClose = false;
    private readonly System.Timers.Timer logAppendTimer = new System.Timers.Timer(500);
    private readonly ConcurrentQueue<LogEntry> pendingLogEntries = new ConcurrentQueue<LogEntry>();

    private static string cacheDir
    {
      get
      {
        var rv = Config.cache;
        if (!string.IsNullOrWhiteSpace(rv) && Directory.Exists(rv)) {
          return rv;
        }
        return Path.GetTempPath();
      }
    }

    public FormMain()
    {
      HandleCreated += (o, e) =>
      {
        logging = true;
      };
      HandleDestroyed += (o, e) =>
      {
        logging = false;
      };

      InitializeComponent();
      listImages.Images.Add("server", Properties.Resources.server);
      listImages.Images.Add("active", Properties.Resources.active);
      listImages.Images.Add("inactive", Properties.Resources.inactive);
      listImages.Images.Add("refreshing", Properties.Resources.refreshing);

      logAppendTimer.Elapsed += (s, e) =>
      {
        BeginInvoke((Action)(() =>
        {
          DoAppendInternal(s, e);
        }));
      };

      SetupLogging();

      StartPipeNotification();

      notifyIcon.Icon = Icon;
      if (!string.IsNullOrWhiteSpace(Config.cache)) {
        cacheFile = new FileInfo(Config.cache);
      }
    }

    public override string Text
    {
      get
      {
        return base.Text;
      }
      set
      {
        base.Text = value;
        notifyIcon.Text = value;
      }
    }

    private void SetupLogging()
    {
      if (!Config.filelogging) {
        BasicConfigurator.Configure(this);
        return;
      }

      var layout = new PatternLayout()
      {
        ConversionPattern = "%date %6level [%3thread] %-14.14logger{1} - %message%newline%exception"
      };
      layout.ActivateOptions();
      var fileAppender = new RollingFileAppender()
      {
        File = logFile.FullName,
        Layout = layout,
        MaximumFileSize = "5MB",
        MaxSizeRollBackups = 3,
        RollingStyle = RollingFileAppender.RollingMode.Size
      };
      fileAppender.ActivateOptions();

      BasicConfigurator.Configure(this, fileAppender);
    }

    private void StartPipeNotification()
    {
#if DEBUG
      log4net.LogManager.GetLogger(this.GetType()).Info("Debug mode / Skipping one-instance-only stuff");
#else
      new Thread(() =>
      {
        for (; ; ) {
          try {
            using (var pipe = new NamedPipeServerStream("simpledlnagui", PipeDirection.InOut)) {
              pipe.WaitForConnection();
              pipe.ReadByte();
              BeginInvoke((Action)(() =>
              {
                notifyIcon_DoubleClick(null, null);
                BringToFront();
              }));
            }
          }
          catch (Exception) {
          }
        }
      }) { IsBackground = true }.Start();
#endif
    }

    private void ButtonNewServer_Click(object sender, EventArgs e)
    {
      using (var ns = new FormServer()) {
        var rv = ns.ShowDialog();
        if (rv == DialogResult.OK) {
          var item = new ServerListViewItem(httpServer, cacheFile, ns.Description);
          listDescriptions.Items.Add(item);
          item.Load();
          SaveConfig();
        }
      }
    }

    private void ListDescriptions_SelectedIndexChanged(object sender, EventArgs e)
    {
      var enable = listDescriptions.SelectedItems.Count != 0;
      buttonStartStop.Enabled = buttonRemove.Enabled = buttonEdit.Enabled = enable;
      if (enable) {
        buttonStartStop.Text = (listDescriptions.SelectedItems[0] as ServerListViewItem).Description.Active ? "Stop" : "Start";
      }
    }

    private void SaveConfig()
    {
      Config.Descriptors = (from ServerListViewItem item in listDescriptions.Items
                            select item.Description).ToList();
      Config.Save();
    }

    private void LoadConfig()
    {
      var descs = (from d in Config.Descriptors
                   let i = new ServerListViewItem(httpServer, cacheFile, d)
                   select i).ToArray();
      listDescriptions.Items.AddRange(descs);

      Task.Factory.StartNew(() =>
      {
        var po = new ParallelOptions()
        {
          MaxDegreeOfParallelism = Math.Min(4, Environment.ProcessorCount)
        };
        Parallel.ForEach(descs, po, i =>
        {
          i.Load();
        });
      });
    }

    private void ButtonEdit_Click(object sender, EventArgs e)
    {
      var item = listDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      using (var ns = new FormServer(item.Description)) {
        var rv = ns.ShowDialog();
        if (rv == DialogResult.OK) {
          var desc = ns.Description;
          Task.Factory.StartNew(() =>
          {
            item.UpdateInfo(desc);
            SaveConfig();
          });
        }
      }
    }

    private void ButtonStartStop_Click(object sender, EventArgs e)
    {
      var item = listDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      Task.Factory.StartNew(() =>
      {
        item.Toggle();
        BeginInvoke((Action)(() =>
        {
          SaveConfig();
          buttonStartStop.Text = item.Description.Active ? "Stop" : "Start";
        }));
      });
    }

    private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
    {
      Text = "Going down...";
      httpServer.Dispose();
      httpServer = null;
    }

    private delegate void logDelegate(string level, string logger, string msg, string ex);

    public void DoAppend(LoggingEvent loggingEvent)
    {
      if (!logging) {
        return;
      }
      if (loggingEvent.Level < Level.Info) {
        return;
      }
      var cls = loggingEvent.LoggerName;
      cls = cls.Substring(cls.LastIndexOf('.') + 1);
      pendingLogEntries.Enqueue(new LogEntry()
      {
        Class = cls,
        Exception = loggingEvent.GetExceptionString(),
        Level = loggingEvent.Level.DisplayName,
        Message = loggingEvent.RenderedMessage
      });
      lock (logAppendTimer) {
        logAppendTimer.Enabled = true;
      }
    }

    public void DoAppendInternal(object sender, System.Timers.ElapsedEventArgs e)
    {
      lock (logAppendTimer) {
        logAppendTimer.Enabled = false;
      }
      if (!logging) {
        return;
      }
      LogEntry entry;
      int last = -1;
      logger.BeginUpdate();
      try {
        while (pendingLogEntries.TryDequeue(out entry)) {
          if (logger.Items.Count >= 300) {
            logger.Items.RemoveAt(0);
          }
          last = logger.Items.Add(new ListViewItem(new string[] { entry.Level, entry.Class, entry.Message })).Index;
          if (!string.IsNullOrWhiteSpace(entry.Exception)) {
            last = logger.Items.Add(new ListViewItem(new string[] { entry.Level, entry.Class, entry.Exception })).Index;
          }
        }
      }
      finally {
        logger.EndUpdate();
      }
      if (last != -1) {
        logger.EnsureVisible(last);
        colLogLogger.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        colLogMessage.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    private void notifyIcon_DoubleClick(object sender, EventArgs e)
    {
      WindowState = FormWindowState.Normal;
      ShowInTaskbar = true;
      notifyIcon.Visible = false;
    }

    private void FormMain_Resize(object sender, EventArgs e)
    {
      if (WindowState == FormWindowState.Minimized) {
        notifyIcon.Visible = true;
        ShowInTaskbar = false;
      }
    }

    private void exitContextMenuItem_Click(object sender, EventArgs e)
    {
      canClose = true;
      notifyIcon_DoubleClick(sender, e);
      Close();
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !canClose;
      if (!canClose) {
        WindowState = FormWindowState.Minimized;
      }
    }

    private void hideToolStripMenuItem_Click(object sender, EventArgs e)
    {
      WindowState = FormWindowState.Minimized;
    }

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var about = new FormAbout()) {
        about.ShowDialog();
      }
    }

    private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Process.Start(string.Format("http://localhost:{0}/", httpServer.RealPort));
    }

    private void FormMain_Load(object sender, EventArgs e)
    {
      httpServer = new HttpServer((int)Config.port);

      Text = string.Format("{0} - Port {1}", Text, httpServer.RealPort);

      LoadConfig();
    }

    private void buttonRemove_Click(object sender, EventArgs e)
    {
      var item = listDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      if (MessageBox.Show(string.Format("Would you like to remove {0}?", item.Description.Name), "Remove Server", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) {
        return;
      }
      if (item.Description.Active) {
        item.Toggle();
      }
      listDescriptions.Items.Remove(item);
      SaveConfig();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var settings = new FormSettings()) {
        settings.ShowDialog();
        Config.Save();
        SetupLogging();
      }
    }
  }
}
