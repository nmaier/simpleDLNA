using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NMaier.SimpleDlna.Server
{//Logging, 
  public sealed class IPAddressAuthorizer : IHttpAuthorizationMethod
  {
   private static readonly ILogging Logger = Logging.GetLogger<IPAddressAuthorizer>();
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
      : this((from a in addresses select IPAddress.Parse(a)))
    {
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
    {
      if (endPoint == null) {
        return false;
      }
      var addr = endPoint.Address;
      if (addr == null) {
        return false;
      }
      var rv = ips.ContainsKey(addr);
      if (!rv) {
        Logger.DebugFormat("Rejecting {0}. Not in IP whitelist", addr);
      }
      else {
        Logger.DebugFormat("Accepted {0} via IP whitelist", addr);
      }
      return rv;
    }
  }
}
