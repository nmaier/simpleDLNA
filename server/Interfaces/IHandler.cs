
namespace NMaier.sdlna.Server
{
  internal interface IHandler
  {

    IResponse HandleRequest(IRequest request);
  }
}
