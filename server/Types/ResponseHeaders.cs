using System;

namespace NMaier.sdlna.Server
{
  public class ResponseHeaders : RawHeaders
  {

    public ResponseHeaders()
      : base()
    {
      this["Server"] = HttpServer.ServerSignature;
      this["Cache-Control"] = "no-cache";
      this["Date"] = DateTime.Now.ToString("R");
      this["Connection"] = "keep-alive";
    }
  }
}
