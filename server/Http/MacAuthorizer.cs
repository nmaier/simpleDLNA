using System;
using System.Collections.Generic;
using System.Net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public sealed class MacAuthorizer : Logging, IHttpAuthorizationMethod
  {
    private readonly Dictionary<string, object> macs =
      new Dictionary<string, object>();

    public MacAuthorizer(IEnumerable<string> macs)
    {
      if (macs == null) {
        throw new ArgumentNullException(nameof(macs));
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
      DebugFormat(!rv ? "Rejecting {0}. Not in MAC whitelist" : "Accepted {0} via MAC whitelist", mac);
      return rv;
    }
  }
}
