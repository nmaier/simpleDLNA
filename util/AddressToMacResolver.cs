using System;
using System.Collections.Concurrent;
using System.Net;
using System.Text.RegularExpressions;
using System.Net.Sockets;

namespace NMaier.SimpleDlna.Utilities
{
  internal sealed class AddressToMacResolver : Logging
  {
    private static readonly Regex regMac =
      new Regex(@"(?:[0-9A-F]{2}:){5}[0-9A-F]{2}", RegexOptions.Compiled);

    private readonly ConcurrentDictionary<IPAddress, MACInfo> cache =
      new ConcurrentDictionary<IPAddress, MACInfo>();

    public static bool IsAcceptedMac(string mac)
    {
      if (string.IsNullOrWhiteSpace(mac)) {
        return false;
      }
      mac = mac.Trim().ToUpperInvariant();
      return regMac.IsMatch(mac);
    }

    public string Resolve(IPAddress ip)
    {
      try {
        if (ip.AddressFamily != AddressFamily.InterNetwork) {
          throw new NotSupportedException(
            "Addresses other than IPV4 are not supported");
        }
        MACInfo info;
        if (cache.TryGetValue(ip, out info) && info.Fresh > DateTime.Now) {
          DebugFormat("From Cache: {0} -> {1}", ip, info.MAC ?? "<UNKNOWN>");
          return info.MAC;
        }
        var raw = new byte[6];
        var length = 6u;
#pragma warning disable 612,618
        var addr = (UInt32)ip.Address;
#pragma warning restore 612,618
        string mac = null;

        try {
          if (SafeNativeMethods.SendARP(addr, 0, raw, ref length) == 0) {
            mac = string.Format("{0:X}:{1:X}:{2:X}:{3:X}:{4:X}:{5:X}",
              raw[0], raw[1], raw[2], raw[3], raw[4], raw[5]);
          }
        }
        catch (DllNotFoundException) {
          // ignore
        }
        cache.TryAdd(ip, new MACInfo() {
          MAC = mac,
          Fresh = DateTime.Now.AddMinutes(mac != null ? 10 : 1)
        });
        DebugFormat("Retrieved: {0} -> {1}", ip, mac ?? "<UNKNOWN>");
        return mac;
      }
      catch (Exception ex) {
        Warn(string.Format("Failed to resolve {0} to MAC", ip), ex);
        return null;
      }
    }

    private struct MACInfo
    {
      public DateTime Fresh;

      public string MAC;
    }
  }
}
