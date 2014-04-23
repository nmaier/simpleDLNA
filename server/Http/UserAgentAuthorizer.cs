using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Net;

namespace NMaier.SimpleDlna.Server
{
  public sealed class UserAgentAuthorizer : Logging, IHttpAuthorizationMethod
  {
    private readonly Dictionary<string, object> userAgents =
      new Dictionary<string, object>();

    private UserAgentAuthorizer()
    {
    }

    public UserAgentAuthorizer(IEnumerable<string> userAgents)
    {
      if (userAgents == null) {
        throw new ArgumentNullException("userAgents");
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
        throw new ArgumentNullException("headers");
      }
      string ua;
      if (!headers.TryGetValue("User-Agent", out ua)) {
        return false;
      }
      if (string.IsNullOrEmpty(ua)) {
        return false;
      }
      var rv = userAgents.ContainsKey(ua);
      if (!rv) {
        DebugFormat("Rejecting {0}. Not in User-Agent whitelist", ua);
      }
      else {
        DebugFormat("Accepted {0} via User-Agent whitelist", ua);
      }
      return rv;
    }
  }
}
