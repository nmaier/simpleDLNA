using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Text;

namespace NMaier.SimpleDlna.GUI
{
  partial class FormAbout : Form
  {
    public FormAbout()
    {
      InitializeComponent();
      Text = String.Format("About {0}", AssemblyTitle);
      labelProductName.Text = AssemblyTitle;
      labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
      labelCopyright.Text = AssemblyCopyright;
      labelCompanyName.Text = AssemblyCompany;
      textBoxLicense.Text = Encoding.UTF8.GetString(Properties.Resources.LICENSE);
    }


    public static string AssemblyCompany
    {
      get {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (attributes.Length == 0) {
          return string.Empty;
        }
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    public static string AssemblyCopyright
    {
      get {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (attributes.Length == 0) {
          return string.Empty;
        }
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }
    public static string AssemblyTitle
    {
      get {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0) {
          var titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (titleAttribute.Title != string.Empty) {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }
    public static string AssemblyVersion
    {
      get {
        return Assembly.GetExecutingAssembly().GetName().Version.ToString();
      }
    }
  }
}
