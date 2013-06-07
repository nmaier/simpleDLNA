using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace NMaier.SimpleDlna.GUI
{
  [RunInstaller(true)]
  public class PathEnvironmentInstaller : Installer
  {
    private const string keyEnvironmentPath = "EnvironmentPath";
    private const string keyRegPath = "PATH";
    private const string keyRegEnvironment = "Environment";
    private readonly DirectoryInfo directory = new FileInfo(Assembly.GetExecutingAssembly().Location).Directory;
    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
      if (!directory.Exists) {
        return;
      }

      using (var registry = Registry.CurrentUser.OpenSubKey(keyRegEnvironment, true)) {
        var path = registry.GetValue(keyRegPath, string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
        if (path == null) {
          return;
        }
        var exists = from p in path.Split(';')
                     where StringComparer.CurrentCultureIgnoreCase.Equals(p, directory.FullName)
                     select p;
        if (exists.Count() > 0) {
          return;
        }
        stateSaver[keyEnvironmentPath] = path;
        var newpath = directory.FullName;
        if (!string.IsNullOrWhiteSpace(path)) {
          newpath = string.Format("{0};{1}", path, newpath);
        }
        registry.SetValue(keyRegPath, newpath, registry.GetValueKind(keyRegPath));
      }
    }
    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
      if (!directory.Exists) {
        return;
      }

      using (var registry = Registry.CurrentUser.OpenSubKey(keyRegEnvironment, true)) {
        var path = registry.GetValue(keyRegPath, string.Empty, RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
        if (path == null) {
          return;
        }
        var cleaned = string.Join(";", from p in path.Split(';')
                                       where !StringComparer.CurrentCultureIgnoreCase.Equals(p, directory.FullName)
                                       select p);
        if (StringComparer.CurrentCultureIgnoreCase.Equals(path, cleaned)) {
          return;
        }
        registry.SetValue(keyRegPath, cleaned, registry.GetValueKind(keyRegPath));
      }
    }

    public override void Rollback(IDictionary savedState)
    {
      base.Rollback(savedState);
      if (!savedState.Contains(keyEnvironmentPath)) {
        return;
      }
      using (var registry = Registry.CurrentUser.OpenSubKey(keyRegEnvironment, true)) {
        registry.SetValue(keyRegPath, savedState[keyEnvironmentPath], registry.GetValueKind(keyRegPath));
      }
    }
  }
}
