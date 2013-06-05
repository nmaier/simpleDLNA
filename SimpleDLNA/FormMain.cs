using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;
using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Core;
using log4net.Layout;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;
using System.Diagnostics;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormMain : Form, IAppender
  {
    private readonly Properties.Settings Config = Properties.Settings.Default;
    private readonly HttpServer httpServer;
    private readonly ILayout layout;
    private readonly FileInfo cacheFile = new FileInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "sdlna.cache"));
    private bool canClose = false;

    public FormMain()
    {
      InitializeComponent();

      notifyIcon.Icon = Icon;

      var layout = new PatternLayout()
      {
        ConversionPattern = "%6level [%3thread] %-14.14logger{1} - %message%newline%exception"
      };
      layout.ActivateOptions();
      this.layout = layout;
      BasicConfigurator.Configure(this);
      LogManager.GetRepository().Threshold = Level.Info;

      if (!string.IsNullOrWhiteSpace(Config.cache)) {
        cacheFile = new FileInfo(Config.cache);
      }
      httpServer = new HttpServer(Config.port);

      Text = string.Format("{0} - Port {1}", Text, httpServer.RealPort);

      LoadConfig();
    }

    private void ButtonNewServer_Click(object sender, EventArgs e)
    {
      using (var ns = new FormServer()) {
        var rv = ns.ShowDialog();
        if (rv == DialogResult.OK) {
          var item = new ServerListViewItem(httpServer, cacheFile, ns.Description);
          ListDescriptions.Items.Add(item);
          SaveConfig();
        }
      }
    }

    private void ListDescriptions_SelectedIndexChanged(object sender, EventArgs e)
    {
      var enable = ListDescriptions.SelectedItems.Count != 0;
      ButtonStartStop.Enabled = ButtonRemove.Enabled = ButtonEdit.Enabled = enable;
      if (enable) {
        ButtonStartStop.Text = (ListDescriptions.SelectedItems[0] as ServerListViewItem).Description.Active ? "Stop" : "Start";
      }
    }

    private void SaveConfig()
    {
      Config.Descriptors = (from ServerListViewItem item in ListDescriptions.Items
                            select item.Description).ToList();
      Config.Save();
    }
    private void LoadConfig()
    {
      foreach (var d in Config.Descriptors) {
        ListDescriptions.Items.Add(new ServerListViewItem(httpServer, cacheFile, d));
      }
    }

    private void ButtonEdit_Click(object sender, EventArgs e)
    {
      var item = ListDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      using (var ns = new FormServer(item.Description)) {
        var rv = ns.ShowDialog();
        if (rv == DialogResult.OK) {
          item.UpdateInfo(ns.Description);
          SaveConfig();
        }
      }
    }

    private void ButtonStartStop_Click(object sender, EventArgs e)
    {
      var item = ListDescriptions.SelectedItems[0] as ServerListViewItem;
      if (item == null) {
        return;
      }
      item.Toggle();
      SaveConfig();

      ButtonStartStop.Text = item.Description.Active ? "Stop" : "Start";
    }

    private void FormMain_FormClosed(object sender, FormClosedEventArgs e)
    {
      httpServer.Dispose();
      Thread.Sleep(2000);
    }

    public void DoAppend(LoggingEvent loggingEvent)
    {
      using (var tw = new StringWriter()) {
        layout.Format(tw, loggingEvent);
        if (Logger.Items.Count >= 300) {
          Logger.Items.RemoveAt(0);
        }
        Logger.Items.Add(tw.ToString());
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
      using (var w = new StringWriter()) {
        var a = Assembly.GetEntryAssembly();
        var v = a.GetName().Version;
        w.WriteLine("App:    {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
        a = Assembly.GetAssembly(typeof(FileServer));
        v = a.GetName().Version;
        w.WriteLine("Files:  {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
        a = Assembly.GetAssembly(typeof(HttpServer));
        v = a.GetName().Version;
        w.WriteLine("Server: {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
        a = Assembly.GetAssembly(typeof(AttributeCollection));
        v = a.GetName().Version;
        w.WriteLine("Utils:  {0} {1}.{2}.{3}", a.GetName().Name, v.Major, v.Minor, v.Revision);
        w.WriteLine("Http:   {0}", HttpServer.Signature);
        MessageBox.Show(w.ToString(), "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
      }
    }

    private void openInBrowserToolStripMenuItem_Click(object sender, EventArgs e)
    {
      Process.Start(string.Format("http://localhost:{0}/", httpServer.RealPort));
    }
  }
}
