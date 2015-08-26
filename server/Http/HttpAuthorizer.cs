using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Net;

namespace NMaier.SimpleDlna.Server
{//Logging, 
  public sealed class HttpAuthorizer : IHttpAuthorizationMethod, IDisposable
  {
    private static readonly ILogging Logger = Logging.GetLogger<HttpAuthorizer>();
    private readonly List<IHttpAuthorizationMethod> methods = new List<IHttpAuthorizationMethod>();
    private readonly HttpServer server = null;

    public HttpAuthorizer()
    {
    }

    public HttpAuthorizer(HttpServer server)
    {
      if (server == null) {
        throw new ArgumentNullException("server");
      }
      this.server = server;
      server.OnAuthorizeClient += OnAuthorize;
    }

    private void OnAuthorize(object sender, HttpAuthorizationEventArgs e)
    {
      e.Cancel = !Authorize(
        e.Headers,
        e.RemoteEndpoint,
        IP.GetMAC(e.RemoteEndpoint.Address)
        );
    }

    public void AddMethod(IHttpAuthorizationMethod method)
    {
      if (method == null) {
        throw new ArgumentNullException("method");
      }
      methods.Add(method);
    }

    public bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac)
    {
      Logger.NoticeFormat("Authorize:[{0}][{1}][{2}]", endPoint, mac, headers["User-Agent"]);
      if (methods.Count == 0) {
        return true;
      }
      try {
        foreach (var m in methods) {
          if (m.Authorize(headers, endPoint, mac)) {
            return true;
          }
        }
        return false;
      }
      catch (Exception ex) {
        Logger.Error("Failed to authorize", ex);
        return false;
      }
    }

    public void Dispose()
    {
      if (server != null) {
        server.OnAuthorizeClient -= OnAuthorize;
      }
    }
  }
}
