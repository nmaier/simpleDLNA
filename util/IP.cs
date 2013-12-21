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
    private static bool warned = false;


    public static IEnumerable<IPAddress> AllIPAddresses
    {
      get
      {
        try {
          return GetIPsDefault().ToArray();
        }
        catch (Exception ex) {
          if (!warned) {
            LogManager.GetLogger(typeof(IP)).Warn(
              "Failed to retrieve IP addresses the usual way, falling back to naive mode", ex);
            warned = true;
          }
          return GetIPsFallback();
        }
      }
    }
    public static IEnumerable<IPAddress> ExternalIPAddresses
    {
      get
      {
        return from i in AllIPAddresses
               where !IPAddress.IsLoopback(i)
               select i;
      }
    }


    private static IEnumerable<IPAddress> GetIPsDefault()
    {
      foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces()) {
        foreach (var uni in adapter.GetIPProperties().UnicastAddresses) {
          var address = uni.Address;
          if (address.AddressFamily != AddressFamily.InterNetwork) {
            continue;
          }
          yield return address;
        }
      }
    }

    private static IEnumerable<IPAddress> GetIPsFallback()
    {
      var returned = false;
      foreach (var i in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
        LogManager.GetLogger(typeof(IP)).Error(i);
        if (i.AddressFamily == AddressFamily.InterNetwork) {
          returned = true;
          yield return i;
        }
      }
      if (!returned) {
        throw new ApplicationException("No IP");
      }
    }
  }
}
