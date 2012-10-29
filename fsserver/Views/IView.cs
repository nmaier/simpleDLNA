using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
{
  public interface IView : IRepositoryItem
  {
    IMediaFolder Transform(FileServer server, IMediaFolder root);
  }
}
