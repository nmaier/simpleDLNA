using System;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormSettings : NMaier.Windows.Forms.Form
  {
    public FormSettings()
    {
      InitializeComponent();
      Icon = Properties.Resources.preferencesIcon;
    }

    private void buttonBrowseCacheFile_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
        textCacheFile.Text = folderBrowserDialog.SelectedPath;
      }
    }
  }
}
