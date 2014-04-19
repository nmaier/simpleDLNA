using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public sealed class IPAddressAuthorizer : Logging, IHttpAuthorizationMethod
  {
    private readonly Dictionary<IPAddress, object> ips =
      new Dictionary<IPAddress, object>();

    private IPAddressAuthorizer()
    {
    }

    public IPAddressAuthorizer(IEnumerable<IPAddress> addresses)
    {
      if (addresses == null) {
        throw new ArgumentNullException("addresses");
      }
      foreach (var ip in addresses) {
        ips.Add(ip, null);
      }
    }

    public IPAddressAuthorizer(IEnumerable<string> addresses)
      : this((from a in addresses
              select IPAddress.Parse(a)))
    {
    }

    public bool Authorize(IHeaders headers, IPEndPoint ep, string mac)
    {
      if (ep == null) {
        return false;
      }
      var addr = ep.Address;
      if (addr == null) {
        return false;
      }
      var rv = ips.ContainsKey(addr);
      if (!rv) {
        DebugFormat("Rejecting {0}. Not in IP whitelist", addr);
      }
      else {
        DebugFormat("Accepted {0} via IP whitelist", addr);
      }
      return rv;
    }
  }
}
