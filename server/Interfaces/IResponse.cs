using System.IO;

namespace NMaier.sdlna.Server
{
  internal interface IResponse
  {

    Stream Body { get; }

    IHeaders Headers { get; }

    HttpCodes Status { get; }
  }
}
