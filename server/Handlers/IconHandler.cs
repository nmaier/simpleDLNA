
namespace NMaier.sdlna.Server
{
  class IconHandler : IPrefixHandler
  {

    public string Prefix { get { return "/icon/"; } }




    public IResponse HandleRequest(IRequest req)
    {
      var resource = req.Path.Substring(Prefix.Length);
      var isPng = resource.IndexOf("PNG") != -1;
      return new ResourceResponse(HttpCodes.OK, isPng ? "image/png" : "image/jpeg", resource);
    }
  }
}
