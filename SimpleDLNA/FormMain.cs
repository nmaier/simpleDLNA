using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NMaier.SimpleDlna.Server;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormMain : NMaier.Windows.Forms.Form, IAppender, IDisposable
  {
    private const string descriptorFile = "descriptors.xml";

    private bool canClose = false;

    private static readonly Properties.Settings Config =
      Properties.Settings.Default;

    private readonly FileInfo cacheFile =
      new FileInfo(Path.Combine(cacheDir, "sdlna.cache"));

#if DEBUG
    private readonly FileInfo logFile =
      new FileInfo(Path.Combine(cacheDir, "sdlna.dbg.log"));
#else
    private readonly FileInfo logFile =
      new FileInfo(Path.Combine(cacheDir, "sdlna.log"));
#endif

    private readonly object appenderLock = new object();

    private readonly System.Timers.Timer appenderTimer =
      new System.Timers.Timer(2000);

    private readonly ConcurrentQueue<LogEntry> pendingLogEntries =
      new ConcurrentQueue<LogEntry>();

    private static readonly ILog log = LogManager.GetLogger(typeof(FormMain));

    private bool minimized = Config.startminimized;

    private HttpServer httpServer;

    private bool logging = false;

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

      listImages.Images.Add("idle", Properties.Resources.idle);
      listImages.Images.Add("active", Properties.Resources.active);
      listImages.Images.Add("inactive", Properties.Resources.inactive);
      listImages.Images.Add("refreshing", Properties.Resources.refreshing);
      listImages.Images.Add("loading", Properties.Resources.loading);
      listImages.Images.Add("info", Properties.Resources.info);
      listImages.Images.Add("warn", Properties.Resources.warn);
      listImages.Images.Add("error", Properties.Resources.error);
      listImages.Images.Add("server", Properties.Resources.server.ToBitmap());

      appenderTimer.Elapsed += (s, e) =>
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
      CreateHandle();
      SetupServer();
    }

    private delegate void logDelegate(string level, string logger, string msg,
                                      string ex);

    private static string cacheDir
    {
      get
      {
        var rv = Config.cache;
        if (!string.IsNullOrWhiteSpace(rv) && Directory.Exists(rv)) {
          return rv;
        }
        try {
          try {
            rv = Environment.GetFolderPath(
              Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(rv)) {
              throw new IOException("Cannot get LocalAppData");
            }
          }
          catch (Exception) {
            rv = Environment.GetFolderPath(
              Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(rv)) {
              throw new IOException("Cannot get LocalAppData");
            }
          }
          rv = Path.Combine(rv, "SimpleDLNA");
          if (!Directory.Exists(rv)) {
            Directory.CreateDirectory(rv);
          }
          return rv;
        }
        catch (Exception) {
          return Path.GetTempPath();
        }
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

    private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var about = new FormAbout()) {
        about.ShowDialog();
      }
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
            BeginInvoke((Action)(() =>
            {
              SaveConfig();
            }));
          });
        }
      }
    }

    private void ButtonNewServer_Click(object sender, EventArgs e)
    {
      using (var ns = new FormServer()) {
        var rv = ns.ShowDialog();
        if (rv == DialogResult.OK) {
          var item = new ServerListViewItem(
            httpServer, cacheFile, ns.Description);
          listDescriptions.Items.Add(item);
          item.Load();
          SaveConfig();
        }
      }
    }

    private void buttonRemove_Click(object sender, EventArgs e)
    {
      var item = listDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      var dr = MessageBox.Show(
        string.Format("Would you like to remove {0}?", item.Description.Name),
        "Remove Server",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Question);
      if (dr != DialogResult.Yes) {
        return;
      }
      if (item.Description.Active) {
        item.Toggle();
      }
      listDescriptions.Items.Remove(item);
      SaveConfig();
    }

    private void buttonRescan_Click(object sender, EventArgs e)
    {
      try {
        var item = listDescriptions.SelectedItems[0] as ServerListViewItem;
        if (item == null) {
          return;
        }
        item.Rescan();
      }
      catch (Exception ex) {
        MessageBox.Show(
          this, ex.Message, "Error", MessageBoxButtons.OK,
          MessageBoxIcon.Error);
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
          ctxStartStop.Text = buttonStartStop.Text =
            item.Description.Active ? "Stop" : "Start";
          ctxStartStop.Image = buttonStartStop.Image =
            item.Description.Active ?
            Properties.Resources.inactive :
            Properties.Resources.active;
        }));
      });
    }

    private void dropCacheToolStripMenuItem_Click(object sender, EventArgs e)
    {
      var res = MessageBox.Show(
        this,
        "Are you sure you want to drop the cache?",
        "Drop cache",
        MessageBoxButtons.YesNo,
        MessageBoxIcon.Warning);
      if (res != DialogResult.Yes) {
        return;
      }
      var running = (from ServerListViewItem item in listDescriptions.Items
                     where item.Description.Active
                     select item).ToList();
      foreach (var item in running) {
        item.Toggle();
      }
      try {
        if (cacheFile.Exists) {
          cacheFile.Delete();
        }
      }
      catch (Exception ex) {
        log.Error(
          string.Format("Failed to remove cache file {0}", cacheFile.FullName),
          ex);
      }
      foreach (var item in running) {
        item.Toggle();
      }
    }

    private void exitContextMenuItem_Click(object sender, EventArgs e)
    {
      canClose = true;
      notifyIcon_DoubleClick(sender, e);
      Close();
    }

    private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
    {
      Text = "Going down...";
      httpServer.Dispose();
      httpServer = null;
    }

    private void FormMain_FormClosing(object sender, FormClosingEventArgs e)
    {
      e.Cancel = !canClose;
      if (!canClose) {
        WindowState = FormWindowState.Minimized;
      }
    }

    private void FormMain_Resize(object sender, EventArgs e)
    {
      if (WindowState == FormWindowState.Minimized) {
        ShowInTaskbar = false;
        minimized = true;
        Hide();
      }
    }

    private void hideToolStripMenuItem_Click(object sender, EventArgs e)
    {
      WindowState = FormWindowState.Minimized;
    }

    private void homepageToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Process.Start("http://nmaier.github.io/simpleDLNA/");
    }

    private void listDescriptions_DoubleClick(object sender, EventArgs e)
    {
      if (buttonEdit.Enabled) {
        ButtonEdit_Click(sender, e);
      }
      else {
        ButtonNewServer_Click(sender, e);
      }
    }

    private void ListDescriptions_SelectedIndexChanged(object sender,
                                                       EventArgs e)
    {
      var enable = listDescriptions.SelectedItems.Count != 0;
      ctxStartStop.Enabled = ctxRemove.Enabled = ctxEdit.Enabled =
        buttonStartStop.Enabled = buttonRemove.Enabled = buttonEdit.Enabled =
        enable;
      if (enable) {
        var item = (listDescriptions.SelectedItems[0] as ServerListViewItem);
        ctxStartStop.Text = buttonStartStop.Text =
          item.Description.Active ? "Stop" : "Start";
        ctxStartStop.Image = buttonStartStop.Image =
          item.Description.Active ?
          Properties.Resources.inactive :
          Properties.Resources.active;
        ctxRescan.Enabled = buttonRescan.Enabled = item.Description.Active;
      }
      else {
        ctxRescan.Enabled = buttonRescan.Enabled = false;
      }
    }

    private void LoadConfig()
    {
      var descs = LoadDescriptors();
      listDescriptions.Items.AddRange(descs.ToArray());

      Task.Factory.StartNew(() =>
      {
        var po = new ParallelOptions() {
          MaxDegreeOfParallelism = Math.Min(2, Environment.ProcessorCount)
        };
        Parallel.ForEach(descs, po, i =>
        {
          i.Load();
        });
        BeginInvoke((Action)(() =>
        {
          Config.Descriptors.Clear();
          SaveConfig();
        }));
      });
    }

    private ServerListViewItem[] LoadDescriptors()
    {
      List<ServerDescription> rv;
      try {
        var serializer = new XmlSerializer(typeof(List<ServerDescription>));
        using (var reader = new StreamReader(
            Path.Combine(cacheDir, descriptorFile))) {
          rv = serializer.Deserialize(reader) as List<ServerDescription>;
        }
      }
      catch (Exception) {
        rv = Config.Descriptors;
      }
      return (from d in rv
              let i = new ServerListViewItem(httpServer, cacheFile, d)
              select i).ToArray();
    }

    private void notifyContext_Opening(object sender,
                                       System.ComponentModel.CancelEventArgs e)
    {
      var items = new List<ToolStripItem>();
      foreach (ToolStripItem i in notifyContext.Items) {
        if (i.Tag != null) {
          items.Add(i);
        }
      }
      foreach (var i in items) {
        notifyContext.Items.Remove(i);
      }
      items.Clear();
      if (listDescriptions.Items.Count == 0) {
        ContextSeperatorPre.Visible = false;
        return;
      }
      ContextSeperatorPre.Visible = true;
      foreach (ServerListViewItem item in listDescriptions.Items) {
        if (!item.Description.Active) {
          continue;
        }
        var innerItem = item;
        var menuItem =
          new ToolStripMenuItem(String.Format("Rescan {0}", item.Text)) {
            Tag = innerItem,
            Image = Properties.Resources.refreshing
          };
        menuItem.Click += (s, a) =>
        {
          try {
            innerItem.Rescan();
          }
          catch (Exception) {
            // no op
          }
        };
        items.Add(menuItem);
      }
      items.Reverse();
      var idx = notifyContext.Items.IndexOf(ContextSeperatorPre) + 1;
      foreach (var i in items) {
        notifyContext.Items.Insert(idx, i);
      }
    }

    private void notifyIcon_DoubleClick(object sender, EventArgs e)
    {
      minimized = false;
      Show();
      WindowState = FormWindowState.Normal;
      ShowInTaskbar = true;
      if (logger != null && logger.Items.Count > 0) {
        logger.EnsureVisible(logger.Items.Count - 1);
      }
    }

    private void openInBrowserToolStripMenuItem_Click(object sender,
                                                      EventArgs e)
    {
      Process.Start(string.Format(
        "http://localhost:{0}/", httpServer.RealPort));
    }

    private void rescanAllContextMenuItem_Click(object sender, EventArgs e)
    {
      foreach (ServerListViewItem l in listDescriptions.Items) {
        try {
          l.Rescan();
        }
        catch (Exception) {
          // no op
        }
      }
    }

    private void SaveConfig()
    {
      try {
        var descs = (from ServerListViewItem item in listDescriptions.Items
                     select item.Description).ToArray();
        var serializer = new XmlSerializer(descs.GetType());
        var file = new FileInfo(
          Path.Combine(cacheDir, descriptorFile + ".tmp"));
        using (var writer = new StreamWriter(file.FullName)) {
          serializer.Serialize(writer, descs);
        }
        var outfile = Path.Combine(cacheDir, descriptorFile);
        File.Copy(file.FullName, outfile, true);
        file.Delete();
      }
      catch (Exception ex) {
        log.Error("Failed to write descriptors", ex);
      }
      Config.Save();
    }

    private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
    {
      using (var settings = new FormSettings()) {
        settings.ShowDialog();
        Config.Save();
        SetupLogging();
      }
    }

    private void SetupLogging()
    {
      if (!Config.filelogging) {
        BasicConfigurator.Configure(this);
        return;
      }

      var layout = new PatternLayout() {
        ConversionPattern = "%date %6level [%3thread] %-30.30logger{1} - %message%newline%exception"
      };
      layout.ActivateOptions();
      var fileAppender = new RollingFileAppender() {
        File = logFile.FullName,
        Layout = layout,
        MaximumFileSize = "10MB",
        MaxSizeRollBackups = 3,
        RollingStyle = RollingFileAppender.RollingMode.Size,
        ImmediateFlush = false,
        Threshold = Level.Info
      };
      fileAppender.ActivateOptions();

      BasicConfigurator.Configure(this, fileAppender);
    }

    private void SetupServer()
    {
      httpServer = new HttpServer((int)Config.port);
      LoadConfig();
      Text = string.Format("{0} - Port {1}", Text, httpServer.RealPort);
    }

    private void StartPipeNotification()
    {
#if DEBUG
      log.Info("Debug mode / Skipping one-instance-only stuff");
#else
      if (Utilities.SystemInformation.IsRunningOnMono()) {
        // XXX Mono sometimes stack overflows for whatever reason.
        return;
      }
      new Thread(() =>
      {
        for (; ; ) {
          try {
            using (var pipe = new NamedPipeServerStream(
                "simpledlnagui", PipeDirection.InOut)) {
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

    protected override void SetVisibleCore(bool value)
    {
      if (minimized) {
        value = false;
        if (!IsHandleCreated) {
          CreateHandle();
        }
      }
      notifyIcon.Visible = !value;
      base.SetVisibleCore(value);
    }

    public void DoAppend(LoggingEvent loggingEvent)
    {
      if (!logging) {
        return;
      }
      if (loggingEvent == null) {
        return;
      }
      if (loggingEvent.Level < Level.Notice) {
        return;
      }
      var cls = loggingEvent.LoggerName;
      cls = cls.Substring(cls.LastIndexOf('.') + 1);
      var key = "info";
      if (loggingEvent.Level >= Level.Error) {
        key = "error";
      }
      else {
        if (loggingEvent.Level >= Level.Warn) {
          key = "warn";
        }
      }
      pendingLogEntries.Enqueue(new LogEntry() {
        Class = cls,
        Exception = loggingEvent.GetExceptionString(),
        Key = key,
        Message = loggingEvent.RenderedMessage,
        Time = loggingEvent.TimeStamp.ToString("T")
      });
      lock (appenderLock) {
        appenderTimer.Enabled = true;
      }
    }

    public void DoAppendInternal(object sender,
                                 System.Timers.ElapsedEventArgs e)
    {
      lock (appenderLock) {
        appenderTimer.Enabled = false;
      }
      if (!logging) {
        return;
      }
      LogEntry entry;
      ListViewItem last = null;
      logger.BeginUpdate();
      try {
        while (pendingLogEntries.TryDequeue(out entry)) {
          if (logger.Items.Count >= 300) {
            logger.Items.RemoveAt(0);
          }
          last = logger.Items.Add(
            new ListViewItem(new string[] {
              entry.Time, entry.Class, entry.Message
            }));
          last.ImageKey = entry.Key;
          if (!string.IsNullOrWhiteSpace(entry.Exception)) {
            last = logger.Items.Add(
              new ListViewItem(new string[] {
                string.Empty, entry.Class, entry.Exception
              }));
            last.ImageKey = entry.Key;
            last.IndentCount = 1;
          }
        }
      }
      finally {
        logger.EndUpdate();
      }
      if (last != null) {
        logger.EnsureVisible(last.Index);
        colLogTime.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        colLogLogger.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
        colLogMessage.AutoResize(ColumnHeaderAutoResizeStyle.ColumnContent);
      }
    }

    private struct LogEntry
    {
      public string Class;

      public string Exception;

      public string Key;

      public string Message;

      public string Time;
    }
  }
}
