using System;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  public partial class FormSettings : Form
  {
    public FormSettings()
    {
      InitializeComponent();
    }

    private void buttonBrowseCacheFile_Click(object sender, EventArgs e)
    {
      if (folderBrowserDialog.ShowDialog() == DialogResult.OK) {
        textCacheFile.Text = folderBrowserDialog.SelectedPath;
      }
    }
  }
}
