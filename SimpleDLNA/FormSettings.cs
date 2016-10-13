using System;
using System.Windows.Forms;
using NMaier.SimpleDlna.GUI.Properties;
using Form = NMaier.Windows.Forms.Form;
using SystemInformation = NMaier.SimpleDlna.Utilities.SystemInformation;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormSettings : Form
  {
    private const string APP_KEY_NAME = "SimpleDLNA";

    private readonly StartupUtilities startUpUtilities;

    public FormSettings()
    {
      InitializeComponent();
      Icon = Resources.preferencesIcon;

      if (!SystemInformation.IsRunningOnMono()) {
        startUpUtilities = new StartupUtilities(StartupUtilities.StartupUserScope.CurrentUser);
        checkAutoStart.Checked = startUpUtilities.CheckIfRunAtWinBoot(APP_KEY_NAME);
      }
      else {
        checkAutoStart.Visible = false;
      }
    }

    private void buttonBrowseCacheFile_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
        textCacheFile.Text = folderBrowserDialog.SelectedPath;
      }
    }

    private void checkAutoStart_CheckedChanged(object sender, EventArgs e)
    {
      if (checkAutoStart.Checked) {
        startUpUtilities.InstallAutoRun(APP_KEY_NAME);
      }
      else {
        startUpUtilities.UninstallAutoRun(APP_KEY_NAME);
      }
    }
  }
}
