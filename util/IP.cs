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

    public static readonly IPAddress[] AllAddresses = GetAllIPs().ToArray();
    public static readonly IPAddress[] ExternalAddresses = GetExternalIPs().ToArray();
    private static bool warned = false;




    private static IEnumerable<IPAddress> GetAllIPs()
    {
      try {
        return GetIPsDefault().ToArray();
      }
      catch (Exception ex) {
        if (!warned) {
          LogManager.GetLogger(typeof(IP)).Warn("Failed to retrieve IP addresses the usual way, falling back to naive mode", ex);
          warned = true;
        }
        return GetIPsFallback();
      }
    }

    private static IEnumerable<IPAddress> GetExternalIPs()
    {
      return from i in GetAllIPs()
             where !IPAddress.IsLoopback(i)
             select i;
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
      bool returned = false;
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
