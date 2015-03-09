using System;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormSettings : NMaier.Windows.Forms.Form
  {

      StartUpUtilities startUpUtilities;
      private const string AppKeyName = "SimpleDLNA";

    public FormSettings()
    {
      InitializeComponent();
      Icon = Properties.Resources.preferencesIcon;

      if(!StartUpUtilities.IsRunningOnMono())
      {
          startUpUtilities = new StartUpUtilities(false);
          checkAutoStart.Checked = startUpUtilities.CheckIfRunAtWinBoot(AppKeyName);
      }
      else
      {
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
        if(checkAutoStart.Checked)
        {
            startUpUtilities.setWinBoot(AppKeyName);
        }
        else
        {
            startUpUtilities.RemoveWinBoot(AppKeyName);
        }
    }
  }
}
