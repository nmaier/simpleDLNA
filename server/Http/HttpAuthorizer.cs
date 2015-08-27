using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server.Http
{//Logging, 
  public sealed class HttpAuthorizer : IHttpAuthorizationMethod, IDisposable
  {
    private static readonly ILogging _logger = Logging.GetLogger<HttpAuthorizer>();
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
      e.Cancel = !Authorize(new HttpRequestAuthParameters(e.Headers,e.RemoteEndpoint)
        //e.Headers,
        //e.RemoteEndpoint,
        //IP.GetMAC(e.RemoteEndpoint.Address)
        );
    }

    public void AddMethod(IHttpAuthorizationMethod method)
    {
      if (method == null) {
        throw new ArgumentNullException("method");
      }
      methods.Add(method);
    }

    public bool Authorize(HttpRequestAuthParameters ap)//IHeaders headers, IPEndPoint endPoint, string mac)
    {
      _logger.NoticeFormat("Authorize:[{0}]", ap);
      if (methods.Count == 0) {
        return true;
      }
      try {
        foreach (var m in methods) {
          if (m.Authorize(ap)) {
            return true;
          }
        }
        return false;
      }
      catch (Exception ex) {
        _logger.Error("Failed to authorize", ex);
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
