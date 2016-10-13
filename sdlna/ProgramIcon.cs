using System;
using System.Reflection;
using System.Runtime.InteropServices;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna
{
  internal class ProgramIcon : Logging, IDisposable
  {
    private readonly IntPtr oldLg = IntPtr.Zero;
    private readonly IntPtr oldSm = IntPtr.Zero;
    private IntPtr window = IntPtr.Zero;

    public ProgramIcon()
    {
      try {
        window = SafeNativeMethods.GetConsoleWindow();
        if (window == IntPtr.Zero) {
          throw new Exception("Cannot get console window");
        }
        var inst = Marshal.GetHINSTANCE(
          Assembly.GetEntryAssembly().GetModules()[0]);
        var iconLg = SafeNativeMethods.LoadImage(inst, "#32512", 1, 0, 0, 0x40);
        if (iconLg == IntPtr.Zero) {
          throw new Exception("Failed to load large icon");
        }
        var desired = SafeNativeMethods.GetSystemMetrics(49);
        var iconSm = SafeNativeMethods.LoadImage(
          inst, "#32512", 1, desired, desired, 0);
        if (iconLg == IntPtr.Zero) {
          throw new Exception("Failed to load small icon");
        }
        oldLg = SafeNativeMethods.SendMessage(
          window, SafeNativeMethods.WM_SETICON, new IntPtr(1), iconLg);
        oldSm = SafeNativeMethods.SendMessage(
          window, SafeNativeMethods.WM_SETICON, IntPtr.Zero, iconSm);
      }
      catch (Exception ex) {
        Debug("Couldnd't set icon", ex);
      }
    }

    public void Dispose()
    {
      GC.SuppressFinalize(this);
      try {
        if (window == IntPtr.Zero) {
          return;
        }
        if (oldLg != IntPtr.Zero) {
          SafeNativeMethods.SendMessage(
            window, SafeNativeMethods.WM_SETICON, new IntPtr(1), oldLg);
        }
        if (oldSm != IntPtr.Zero) {
          SafeNativeMethods.SendMessage(
            window, SafeNativeMethods.WM_SETICON, IntPtr.Zero, oldSm);
        }
        window = IntPtr.Zero;
      }
      catch (Exception ex) {
        Debug("Couldn't restore icon", ex);
      }
    }

    ~ProgramIcon()
    {
      Dispose();
    }
  }
}
