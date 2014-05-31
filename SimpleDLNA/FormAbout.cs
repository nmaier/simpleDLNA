using NMaier.SimpleDlna.Utilities;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  partial class FormAbout : Form
  {
    public FormAbout()
    {
      InitializeComponent();
      FormMain.SetFlatStyle(this);
      Text = String.Format("About {0}", ProductInformation.Title);
      Product.Text = ProductInformation.Title;
      Product.Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Bold);
      Version.Text = String.Format("Version {0}", ProductInformation.ProductVersion);
      Copyright.Text = ProductInformation.Copyright;
      Copyright.Font = new Font(SystemFonts.MessageBoxFont, FontStyle.Italic);
      License.Text = Encoding.UTF8.GetString(Properties.Resources.LICENSE);
    }
  }
}
