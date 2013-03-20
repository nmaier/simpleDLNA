using System.IO;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaResource : IMediaItem
  {
    Stream Content { get; }
    DlnaMediaTypes MediaType { get; }
    string PN { get; }
    DlnaMime Type { get; }
  }
}
