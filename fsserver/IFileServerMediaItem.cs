using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal interface IFileServerMediaItem : IMediaItem
  {
    string Id { get; set; }
    string Path { get; }
  }
}
