using System;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormSettings : NMaier.Windows.Forms.Form
  {
    private const string AppKeyName = "SimpleDLNA";

    private StartupUtilities startUpUtilities;

    public FormSettings()
    {
      InitializeComponent();
      Icon = Properties.Resources.preferencesIcon;

      if (!Utilities.SystemInformation.IsRunningOnMono()) {
        startUpUtilities = new StartupUtilities(StartupUtilities.StartupUserScope.CurrentUser);
        checkAutoStart.Checked = startUpUtilities.CheckIfRunAtWinBoot(AppKeyName);
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
        startUpUtilities.InstallAutoRun(AppKeyName);
      }
      else {
        startUpUtilities.UninstallAutoRun(AppKeyName);
      }
    }
  }
}
