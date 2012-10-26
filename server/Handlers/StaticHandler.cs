
namespace NMaier.sdlna.Server
{
  internal sealed class StaticHandler : IPrefixHandler
  {

    private readonly string prefix;
    private readonly IResponse response;



    public StaticHandler(string aPrefix, IResponse aResponse)
    {
      prefix = aPrefix;
      response = aResponse;
    }

    public StaticHandler(IResponse aResponse)
      : this("#", aResponse)
    {
    }



    public string Prefix
    {
      get { return prefix; }
    }




    public IResponse HandleRequest(IRequest req)
    {
      return response;
    }
  }
}