using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  interface IFileServerFolder : IMediaFolder, IFileServerMediaItem
  {

    FileServer Server { get; }

    void AdoptItem(IFileServerMediaItem item);

    void ReleaseItem(IFileServerMediaItem item);

    void Sort(Comparers.IItemComparer comparer, bool descending);
  }
}
