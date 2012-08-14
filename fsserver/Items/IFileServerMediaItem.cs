using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  interface IFileServerMediaItem : IMediaItem
  {

    new string ID { get; set; }

    new IMediaFolder Parent { get; set; }

    string Path { get; }
  }
}