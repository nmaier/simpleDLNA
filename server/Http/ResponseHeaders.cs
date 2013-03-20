using System;

namespace NMaier.SimpleDlna.Server
{
  public sealed class ResponseHeaders : RawHeaders
  {
    public ResponseHeaders()
    {
      this["Server"] = HttpServer.Signature;
      this["Cache-Control"] = "no-cache";
      this["Date"] = DateTime.Now.ToString("R");
      this["Connection"] = "keep-alive";
    }
  }
}
