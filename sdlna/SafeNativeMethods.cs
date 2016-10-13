using System;
using System.Runtime.InteropServices;

namespace NMaier.SimpleDlna
{
  internal static class SafeNativeMethods
  {
    public const int WM_SETICON = 0x0080;

    [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern int GetSystemMetrics(int index);

    [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    public static extern IntPtr LoadImage(IntPtr inst, string name,
      uint type, int cxDesired,
      int cyDesired, uint fuLoad);

    [DllImport("user32.dll", CharSet = CharSet.Auto)]
    public static extern IntPtr SendMessage(IntPtr hWnd, int msg,
      IntPtr wParam, IntPtr lParam);
  }
}
