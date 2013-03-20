using System.IO;

namespace NMaier.SimpleDlna.Server
{
  internal interface IResponse
  {
    Stream Body { get; }
    IHeaders Headers { get; }
    HttpCodes Status { get; }
  }
}
