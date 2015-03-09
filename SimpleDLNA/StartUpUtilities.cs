using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  public class StartupUtilities : IDisposable
  {
    private readonly RegistryKey appKey;

    public StartupUtilities(StartupUserScope userScope)
    {
      switch (userScope) {
        default:
        case StartupUserScope.CurrentUser:
          appKey = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
          break;
        case StartupUserScope.AllUsers:
          appKey = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
          break;
      }
    }

    public enum StartupUserScope
    {
      AllUsers,
      CurrentUser
    }

    /// <summary>
    /// Checks for autorun registry entry
    /// </summary>
    /// <param name="AppName">"key" to search for in startup registry</param>
    /// <returns>True if app is set to start with windows.</returns>
    public bool CheckIfRunAtWinBoot(string AppName)
    {
      // if the key exists and has a value, then autostart is enabled
      return appKey.GetValue(AppName) != null;
    }

    public void Dispose()
    {
      if (appKey != null) {
        appKey.Dispose();
      }
    }

    /// <summary>
    /// Sets application to run at windows boot, using an AppName as the registry key name, current executable path
    /// </summary>
    /// <param name="AppName">Name the startup key will use, should be unique</param>
    public void InstallAutoRun(string AppName)
    {
      InstallAutoRun(AppName, Application.ExecutablePath);
    }

    /// <summary>
    /// Sets application to run at windows boot, using an AppName as the registry key name, and any supplied path
    /// </summary>
    /// <param name="AppName">Name the startup key will use, should be unique</param>
    /// <param name="AppPath">Path to the executable to run</param>
    public void InstallAutoRun(string AppName, string AppPath)
    {
      appKey.SetValue(AppName, AppPath);
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="AppName"></param>
    public void UninstallAutoRun(string AppName)
    {
      appKey.DeleteValue(AppName, false);
    }
  }
}
