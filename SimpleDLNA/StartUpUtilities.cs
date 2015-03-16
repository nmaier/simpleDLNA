using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
    public class StartUpUtilities
    {
        private readonly RegistryKey rkApp;

        public enum StartupUserScope
        {
            CurrentUser,
            AllUsers
        }

        public StartUpUtilities(StartupUserScope userScope)
        {
            switch (userScope){
                default:
                case StartupUserScope.CurrentUser:
                    rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    break;
                case StartupUserScope.AllUsers:
                    rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                    break;
            }

        }

        /// <summary>
        /// Checks for autorun registry entry
        /// </summary>
        /// <param name="AppName">"key" to search for in startup registry</param>
        /// <returns>True if app is set to start with windows.</returns>
        public bool CheckIfRunAtWinBoot(string AppName)
        {
            // if the key exists and has a value, then autostart is enabled
            return rkApp.GetValue(AppName) != null;
        }

        /// <summary>
        /// Sets application to run at windows boot, using an AppName as the registry key name, and any supplied path
        /// </summary>
        /// <param name="AppName">Name the startup key will use, should be unique</param>
        /// <param name="AppPath">Path to the executable to run</param>
        public void InstallAutoRun(string AppName, string AppPath)
        {
            rkApp.SetValue(AppName, AppPath);
        }

        /// <summary>
        /// Sets application to run at windows boot, using an AppName as the registry key name, current executable path
        /// </summary>
        /// <param name="AppName">Name the startup key will use, should be unique</param>
        public void InstallAutoRun(string AppName)
        {
            this.InstallAutoRun(AppName, Application.ExecutablePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppName"></param>
        public void UninstallAutoRun(string AppName)
        {
            rkApp.DeleteValue(AppName, false);
        }

    }
}
