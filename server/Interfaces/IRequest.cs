using System.Net;

namespace NMaier.sdlna.Server
{
  public interface IRequest
  {

    string Body { get; }

    IPEndPoint Endpoint { get; }

    IHeaders Headers { get; }

    string Method { get; }

    string Path { get; }
  }
}
