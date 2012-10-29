using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  interface IFileServerMediaItem : IMediaItem
  {

    new string Id { get; set; }

    string Path { get; }
  }
}