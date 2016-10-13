using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using log4net;

namespace NMaier.SimpleDlna.Utilities
{
  public static class IP
  {
    private static readonly AddressToMacResolver macResolver =
      new AddressToMacResolver();

    private static readonly ILog logger = LogManager.GetLogger(typeof (IP));

    private static bool warned;

    public static IEnumerable<IPAddress> AllIPAddresses
    {
      get {
        try {
          return GetIPsDefault().ToArray();
        }
        catch (Exception ex) {
          if (!warned) {
            logger.Warn(
              "Failed to retrieve IP addresses the usual way, falling back to naive mode",
              ex);
            warned = true;
          }
          return GetIPsFallback();
        }
      }
    }

    public static IEnumerable<IPAddress> ExternalIPAddresses => from i in AllIPAddresses
                                                                where !IPAddress.IsLoopback(i)
                                                                select i;

    private static IEnumerable<IPAddress> GetIPsDefault()
    {
      var returned = false;
      foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces()) {
        var props = adapter.GetIPProperties();
        var gateways = from ga in props.GatewayAddresses
                       where !ga.Address.Equals(IPAddress.Any)
                       select true;
        if (!gateways.Any()) {
          logger.DebugFormat("Skipping {0}. No gateways", props);
          continue;
        }
        logger.DebugFormat("Using {0}", props);
        foreach (var uni in props.UnicastAddresses) {
          var address = uni.Address;
          if (address.AddressFamily != AddressFamily.InterNetwork) {
            logger.DebugFormat("Skipping {0}. Not IPv4", address);
            continue;
          }
          logger.DebugFormat("Found {0}", address);
          returned = true;
          yield return address;
        }
      }
      if (!returned) {
        throw new ApplicationException("No IP");
      }
    }

    private static IEnumerable<IPAddress> GetIPsFallback()
    {
      var returned = false;
      foreach (var i in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
        if (i.AddressFamily == AddressFamily.InterNetwork) {
          logger.DebugFormat("Found {0}", i);
          returned = true;
          yield return i;
        }
      }
      if (!returned) {
        throw new ApplicationException("No IP");
      }
    }

    public static string GetMAC(IPAddress address)
    {
      return macResolver.Resolve(address);
    }

    public static bool IsAcceptedMAC(string mac)
    {
      return AddressToMacResolver.IsAcceptedMac(mac);
    }
  }
}
