using System.IO;

namespace NMaier.sdlna.Server
{
  public interface IMediaResource : IMediaItem
  {

    Stream Content { get; }

    string PN { get; }
    MediaTypes MediaType { get; }


    DlnaTypes Type { get; }
  }
}