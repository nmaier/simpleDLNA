using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NMaier.SimpleDlna.Utilities
{
  public class DataPath
  {
    private static string _workPath = GetSystemWorkPath();

    public static string Path {
      get { return _workPath; }
      set { _workPath = value; }
    }

    public static string Combine(params string[] paths) {
      return System.IO.Path.Combine((new [] { _workPath }).Concat(paths).ToArray());
    }

    private static string GetSystemWorkPath()
    {
      string rv;
        try {
          try {
            rv = Environment.GetFolderPath(
              Environment.SpecialFolder.LocalApplicationData);
            if (string.IsNullOrEmpty(rv)) {
              throw new IOException("Cannot get LocalAppData");
            }
          }
          catch (Exception) {
            rv = Environment.GetFolderPath(
              Environment.SpecialFolder.ApplicationData);
            if (string.IsNullOrEmpty(rv)) {
              throw new IOException("Cannot get LocalAppData");
            }
          }
          rv = System.IO.Path.Combine(rv, "SimpleDLNA");
          if (!Directory.Exists(rv)) {
            Directory.CreateDirectory(rv);
          }
          return rv;
        }
        catch (Exception) {
          return System.IO.Path.GetTempPath();
        }
      }
  }
}
