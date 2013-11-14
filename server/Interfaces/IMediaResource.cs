using System.IO;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaResource : IMediaItem, IMediaCover
  {
    Stream Content { get; }
    DlnaMediaTypes MediaType { get; }
    string PN { get; }
    DlnaMime Type { get; }
  }
}
