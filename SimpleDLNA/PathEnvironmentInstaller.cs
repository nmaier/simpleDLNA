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
    private const string ENV_PATH = "EnvironmentPath";

    private const string REG_ENV = "Environment";

    private const string REG_PATH = "PATH";

    private readonly DirectoryInfo directory =
      new FileInfo(Assembly.GetExecutingAssembly().Location ?? string.Empty).Directory;

    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
      if (!directory.Exists) {
        return;
      }

      using (var registry = Registry.CurrentUser.OpenSubKey(REG_ENV, true)) {
        var path = registry?.GetValue(
          REG_PATH, string.Empty,
          RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
        if (path == null) {
          return;
        }
        var c = StringComparer.CurrentCultureIgnoreCase;
        var exists = from p in path.Split(';')
                     where c.Equals(p, directory.FullName)
                     select p;
        if (exists.Any()) {
          return;
        }
        stateSaver[ENV_PATH] = path;
        var newpath = directory.FullName;
        if (!string.IsNullOrWhiteSpace(path)) {
          newpath = $"{path};{newpath}";
        }
        registry.SetValue(REG_PATH, newpath, RegistryValueKind.ExpandString);
      }
    }

    public override void Rollback(IDictionary savedState)
    {
      base.Rollback(savedState);
      if (!savedState.Contains(ENV_PATH)) {
        return;
      }
      using (var registry = Registry.CurrentUser.OpenSubKey(REG_ENV, true)) {
        registry?.SetValue(
          REG_PATH, savedState[ENV_PATH], registry.GetValueKind(REG_PATH));
      }
    }

    public override void Uninstall(IDictionary savedState)
    {
      base.Uninstall(savedState);
      if (!directory.Exists) {
        return;
      }

      using (var registry = Registry.CurrentUser.OpenSubKey(REG_ENV, true)) {
        var path = registry?.GetValue(
          REG_PATH, string.Empty,
          RegistryValueOptions.DoNotExpandEnvironmentNames) as string;
        if (string.IsNullOrEmpty(path)) {
          return;
        }
        var c = StringComparer.CurrentCultureIgnoreCase;
        var paths = from p in path.Split(';')
                    where !c.Equals(p, directory.FullName)
                    select p;
        var cleaned = string.Join(";", paths);
        if (StringComparer.CurrentCultureIgnoreCase.Equals(path, cleaned)) {
          return;
        }
        registry.SetValue(REG_PATH, cleaned, registry.GetValueKind(REG_PATH));
      }
    }
  }
}
