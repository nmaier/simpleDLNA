using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NMaier.SimpleDlna.Server.Http
{//Logging, 
  public sealed class IPAddressAuthorizer : IHttpAuthorizationMethod
  {
   private static readonly ILogging _logger = Logging.GetLogger<IPAddressAuthorizer>();
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

    public bool Authorize(HttpRequestAuthParameters ap)//IHeaders headers, IPEndPoint endPoint, string mac)
    {
      //if (endPoint == null) {
      //  return false;
      //}
      //var addr = endPoint.Address;
      if (ap.Address == null) {
        return false;
      }
      var rv = ips.ContainsKey(ap.Address);
      if (!rv) {
        _logger.DebugFormat("Rejecting {0}. Not in IP whitelist", ap.Address);
      }
      else {
        _logger.DebugFormat("Accepted {0} via IP whitelist", ap.Address);
      }
      return rv;
    }
  }
}
