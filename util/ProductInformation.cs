using System;
using System.Reflection;

namespace NMaier.SimpleDlna.Utilities
{
  public static class ProductInformation
  {
    public static string Company
    {
      get
      {
        var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
        if (attributes.Length == 0) {
          return string.Empty;
        }
        return ((AssemblyCompanyAttribute)attributes[0]).Company;
      }
    }
    public static string Copyright
    {
      get
      {
        var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
        if (attributes.Length == 0) {
          return string.Empty;
        }
        return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
      }
    }
    public static string ProductVersion
    {
      get
      {
        var attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyInformationalVersionAttribute), false);
        if (attributes.Length == 0) {
          return string.Empty;
        }
        return ((AssemblyInformationalVersionAttribute)attributes[0]).InformationalVersion;
      }
    }
    public static string Title
    {
      get
      {
        var attributes = Assembly.GetEntryAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
        if (attributes.Length > 0) {
          var titleAttribute = (AssemblyTitleAttribute)attributes[0];
          if (!string.IsNullOrWhiteSpace(titleAttribute.Title)) {
            return titleAttribute.Title;
          }
        }
        return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
      }
    }
  }
}
