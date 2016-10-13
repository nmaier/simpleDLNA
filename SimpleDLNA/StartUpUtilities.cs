using System;
using System.Windows.Forms;
using Microsoft.Win32;

namespace NMaier.SimpleDlna.GUI
{
  public class StartupUtilities : IDisposable
  {
    public enum StartupUserScope
    {
      AllUsers,
      CurrentUser
    }

    private readonly RegistryKey appKey;

    public StartupUtilities(StartupUserScope userScope)
    {
      switch (userScope) {
      default:
        appKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        break;
      case StartupUserScope.AllUsers:
        appKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
        break;
      }
    }

    public void Dispose()
    {
      appKey?.Dispose();
    }

    /// <summary>
    ///   Checks for autorun registry entry
    /// </summary>
    /// <param name="name">"key" to search for in startup registry</param>
    /// <returns>True if app is set to start with windows.</returns>
    public bool CheckIfRunAtWinBoot(string name)
    {
      // if the key exists and has a value, then autostart is enabled
      return appKey.GetValue(name) != null;
    }

    /// <summary>
    ///   Sets application to run at windows boot, using an name as the registry key name, current executable path
    /// </summary>
    /// <param name="name">Name the startup key will use, should be unique</param>
    public void InstallAutoRun(string name)
    {
      InstallAutoRun(name, Application.ExecutablePath);
    }

    /// <summary>
    ///   Sets application to run at windows boot, using an name as the registry key name, and any supplied path
    /// </summary>
    /// <param name="name">Name the startup key will use, should be unique</param>
    /// <param name="path">Path to the executable to run</param>
    public void InstallAutoRun(string name, string path)
    {
      appKey.SetValue(name, path);
    }

    /// <summary>
    /// </summary>
    /// <param name="name"></param>
    public void UninstallAutoRun(string name)
    {
      appKey.DeleteValue(name, false);
    }
  }
}
