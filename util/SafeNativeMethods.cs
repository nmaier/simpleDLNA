using System;
using System.Runtime.InteropServices;

namespace NMaier.SimpleDlna.Utilities
{
  internal static class SafeNativeMethods
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    internal static extern int StrCmpLogicalW(string psz1, string psz2);

    [DllImport("iphlpapi.dll")]
    public static extern UInt32 SendARP(UInt32 DestIP, UInt32 SrcIP, [Out] byte[] pMacAddr, ref UInt32 PhyAddrLen);
  }
}
