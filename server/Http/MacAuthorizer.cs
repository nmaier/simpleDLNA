using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Net;

namespace NMaier.SimpleDlna.Server
{//Logging, 
  public sealed class MacAuthorizer : IHttpAuthorizationMethod
  {
   private static readonly ILogging Logger = Logging.GetLogger<MacAuthorizer>();
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

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
    {
      if (string.IsNullOrEmpty(mac)) {
        return false;
      }

      var rv = macs.ContainsKey(mac);
      if (!rv) {
        Logger.DebugFormat("Rejecting {0}. Not in MAC whitelist", mac ?? "<UNKNOWN>");
      }
      else {
        Logger.DebugFormat("Accepted {0} via MAC whitelist", mac);
      }
      return rv;
    }
  }
}
