using System;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class IconHandler : IPrefixHandler
  {
    public string Prefix => "/icon/";

    public IResponse HandleRequest(IRequest req)
    {
      var resource = req.Path.Substring(Prefix.Length);
      var isPNG = resource.EndsWith(
        ".png", StringComparison.OrdinalIgnoreCase);
      return new ResourceResponse(
        HttpCode.Ok,
        isPNG ? "image/png" : "image/jpeg",
        resource
        );
    }
  }
}
