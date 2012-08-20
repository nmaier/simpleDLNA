using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  interface IFileServerFolder : IMediaFolder, IFileServerMediaItem
  {

    FileServer Server { get; }

    void AdoptItem(IFileServerMediaItem item);

    void ReleaseItem(IFileServerMediaItem item);

    void Sort(IItemComparer comparer, bool descending);
  }
}
