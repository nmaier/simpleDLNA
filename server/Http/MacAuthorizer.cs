using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server.Http
{//Logging, 
  public sealed class MacAuthorizer : IHttpAuthorizationMethod
  {
    private static readonly ILogging _logger = Logging.GetLogger<MacAuthorizer>();
    private readonly Dictionary<string, object> macs =
      new Dictionary<string, object>();

    private MacAuthorizer()
    {
    }

    public MacAuthorizer(IEnumerable<string> macs)
    {
      if (macs == null) {
        throw new ArgumentNullException("macs");
      }
      foreach (var m in macs) {
        var mac = m.ToUpperInvariant().Trim();
        if (!IP.IsAcceptedMAC(mac)) {
          throw new FormatException("Invalid MAC supplied");
        }
        this.macs.Add(mac, null);
      }
    }

    public bool Authorize(HttpRequestAuthParameters ap)//IHeaders headers, IPEndPoint endPoint, string mac)
    {
      if (string.IsNullOrEmpty(ap.Mac)) {
        return false;
      }

      var rv = macs.ContainsKey(ap.Mac);
      if (!rv) {
        _logger.DebugFormat("Rejecting {0}. Not in MAC whitelist", ap.Mac ?? "<UNKNOWN>");
      }
      else {
        _logger.DebugFormat("Accepted {0} via MAC whitelist", ap.Mac);
      }
      return rv;
    }
  }
}
