using NMaier.SimpleDlna.Utilities;
using System;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  partial class FormAbout : NMaier.Windows.Forms.Form
  {
    public FormAbout()
    {
      InitializeComponent();
      Text = String.Format("About {0}", ProductInformation.Title);
      Product.Text = ProductInformation.Title;
      Product.Font = BoldFont;
      Version.Text = String.Format(
        "Version {0}", ProductInformation.ProductVersion);
      Copyright.Text = ProductInformation.Copyright;
      Copyright.Font = ItalicFont;
      License.Text = Encoding.UTF8.GetString(Properties.Resources.LICENSE);
    }
  }
}
