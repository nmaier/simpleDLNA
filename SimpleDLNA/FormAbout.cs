using System.Text;
using NMaier.SimpleDlna.GUI.Properties;
using NMaier.SimpleDlna.Utilities;
using NMaier.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  internal sealed partial class FormAbout : Form
  {
    public FormAbout()
    {
      InitializeComponent();
      Text = $"About {ProductInformation.Title}";
      Product.Text = ProductInformation.Title;
      Product.Font = BoldFont;
      Version.Text = $"Version {ProductInformation.ProductVersion}";
      Copyright.Text = ProductInformation.Copyright;
      Copyright.Font = ItalicFont;
      License.Text = Encoding.UTF8.GetString(Resources.LICENSE);
    }
  }
}
