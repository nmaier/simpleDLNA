using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
    class StartUpUtilities
    {
        RegistryKey rkApp;

        public StartUpUtilities(bool AllUsers)
        {
            if (AllUsers)
            {
                // The path to the key where Windows looks for startup applications for all users
                rkApp = Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            }
            else
            {
                // The path to the key where Windows looks for startup applications for single user
                rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppName">"key" to search for in startup registry</param>
        /// <returns>True if app is set to start with windows.</returns>
        public bool CheckIfRunAtWinBoot(string AppName)
        {
            // Check to see the current state (running at startup or not)
            if (rkApp.GetValue(AppName) == null)
            {
                // The value doesn't exist, the application is not set to run at startup
                return false;
            }

            else
            {
                // The value exists, the application is set to run at startup
                return true;
            }
        }

        /// <summary>
        /// Sets application to run at windows boot, using an AppName as the registry key name, and any supplied path
        /// </summary>
        /// <param name="AppName">Name the startup key will use, should be unique</param>
        /// <param name="AppPath">Path to the executable to run</param>
        public void setWinBoot(string AppName, string AppPath)
        {
            rkApp.SetValue(AppName, AppPath);
        }

        /// <summary>
        /// Sets application to run at windows boot, using an AppName as the registry key name, current executable path
        /// </summary>
        /// <param name="AppName">Name the startup key will use, should be unique</param>
        public void setWinBoot(string AppName)
        {
            rkApp.SetValue(AppName, Application.ExecutablePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="AppName"></param>
        public void RemoveWinBoot(string AppName)
        {
            rkApp.DeleteValue(AppName, false);
        }

        /// <summary>
        /// Returns true if applicaton is running under mono
        /// </summary>
        public static bool IsRunningOnMono()
        {
            return Type.GetType("Mono.Runtime") != null;
        }
    }
}
