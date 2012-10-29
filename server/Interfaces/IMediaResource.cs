using System.IO;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaResource : IMediaItem
  {

    Stream Content { get; }

    MediaTypes MediaType { get; }

    string PN { get; }

    DlnaType Type { get; }
  }
}