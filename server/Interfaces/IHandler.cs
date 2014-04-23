namespace NMaier.SimpleDlna.Server
{
  internal interface IHandler
  {
    IResponse HandleRequest(IRequest request);
  }
}
