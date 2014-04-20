using System;
using System.Text;
using System.Windows.Forms;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.GUI
{
  partial class FormAbout : Form
  {
    public FormAbout()
    {
      InitializeComponent();
      Text = String.Format("About {0}", ProductInformation.Title);
      Product.Text = ProductInformation.Title;
      Version.Text = String.Format("Version {0}", ProductInformation.ProductVersion);
      Copyright.Text = ProductInformation.Copyright;
      License.Text = Encoding.UTF8.GetString(Properties.Resources.LICENSE);
    }
  }
}
