using System;

namespace NMaier.SimpleDlna.Server.Http
{
  public sealed class ResponseHeaders : RawHeaders
  {
    public ResponseHeaders()
      : this(noCache: true)
    {
    }

    public ResponseHeaders(bool noCache)
    {
      this["Server"] = HttpServer.Signature;
      this["Date"] = DateTime.Now.ToString("R");
      this["Connection"] = "keep-alive";
      if (noCache) {
        this["Cache-Control"] = "no-cache";
      }
    }
  }
}
