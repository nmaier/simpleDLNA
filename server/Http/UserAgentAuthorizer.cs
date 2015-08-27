using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server.Http
{//Logging, 
  public sealed class UserAgentAuthorizer : IHttpAuthorizationMethod
  {
   private static readonly ILogging _logger = Logging.GetLogger<UserAgentAuthorizer>();
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

    public bool Authorize(HttpRequestAuthParameters ap)//IHeaders headers, IPEndPoint endPoint, string mac)
    {
      //if (headers == null) {
      //  throw new ArgumentNullException("headers");
      //}
      //string ua;
      //if (!headers.TryGetValue("User-Agent", out ua)) {
      //  return false;
      //}
      if (string.IsNullOrEmpty(ap.UserAgent)) {
        return false;
      }
      var rv = userAgents.ContainsKey(ap.UserAgent);
      if (!rv) {
        _logger.DebugFormat("Rejecting {0}. Not in User-Agent whitelist", ap.UserAgent);
      }
      else {
        _logger.DebugFormat("Accepted {0} via User-Agent whitelist", ap.UserAgent);
      }
      return rv;
    }
  }
}
