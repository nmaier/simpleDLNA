using System;

namespace NMaier.SimpleDlna.Server
{
  public sealed class ResponseHeaders : RawHeaders
  {
    public ResponseHeaders()
      : this(true)
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
