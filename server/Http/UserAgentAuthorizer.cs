using System;
using System.Collections.Generic;
using System.Net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public sealed class UserAgentAuthorizer : Logging, IHttpAuthorizationMethod
  {
    private readonly Dictionary<string, object> userAgents =
      new Dictionary<string, object>();

    public UserAgentAuthorizer(IEnumerable<string> userAgents)
    {
      if (userAgents == null) {
        throw new ArgumentNullException(nameof(userAgents));
      }
      foreach (var u in userAgents) {
        if (string.IsNullOrEmpty(u)) {
          throw new FormatException("Invalid User-Agent supplied");
        }
        this.userAgents.Add(u, null);
      }
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
    {
      if (headers == null) {
        throw new ArgumentNullException(nameof(headers));
      }
      string ua;
      if (!headers.TryGetValue("User-Agent", out ua)) {
        return false;
      }
      if (string.IsNullOrEmpty(ua)) {
        return false;
      }
      var rv = userAgents.ContainsKey(ua);
      DebugFormat(!rv ? "Rejecting {0}. Not in User-Agent whitelist" : "Accepted {0} via User-Agent whitelist", ua);
      return rv;
    }
  }
}
