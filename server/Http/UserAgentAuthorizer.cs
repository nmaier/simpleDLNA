using System;
using System.Collections.Generic;
using System.Net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public sealed class UserAgentAuthorizer : Logging, IHttpAuthorizationMethod
  {
    private readonly Dictionary<string, object> uas = new Dictionary<string, object>();


    private UserAgentAuthorizer()
    {
    }


    public UserAgentAuthorizer(IEnumerable<string> uas)
    {
      foreach (var u in uas) {
        if (string.IsNullOrEmpty(u)) {
          throw new FormatException("Invalid User-Agent supplied");
        }
        this.uas.Add(u, null);
      }
    }


    public bool Authorize(IHeaders headers, IPEndPoint ep, string mac)
    {
      string ua;
      if (!headers.TryGetValue("User-Agent", out ua)) {
        return false;
      }
      if (string.IsNullOrEmpty(ua)) {
        return false;
      }
      var rv = uas.ContainsKey(ua);
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
