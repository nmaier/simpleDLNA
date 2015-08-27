using NMaier.SimpleDlna.Server.Http;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class IconHandler : IPrefixHandler
  {
    public string Prefix
    {
      get
      {
        return "/icon/";
      }
    }

    public IResponse HandleRequest(IRequest req)
    {
      var resource = req.Path.Substring(Prefix.Length);
      var isPng = resource.EndsWith(
        ".png", System.StringComparison.OrdinalIgnoreCase);
      return new ResourceResponse(
        HttpCode.Ok,
        isPng ? "image/png" : "image/jpeg",
        resource
        );
    }
  }
}
