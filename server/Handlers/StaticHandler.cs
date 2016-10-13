namespace NMaier.SimpleDlna.Server
{
  internal sealed class StaticHandler : IPrefixHandler
  {
    private readonly IResponse response;

    public StaticHandler(IResponse aResponse)
      : this("#", aResponse)
    {
    }

    public StaticHandler(string aPrefix, IResponse aResponse)
    {
      Prefix = aPrefix;
      response = aResponse;
    }

    public string Prefix { get; }

    public IResponse HandleRequest(IRequest req)
    {
      return response;
    }
  }
}
