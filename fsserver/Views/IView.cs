using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
{
  public interface IView : IRepositoryItem
  {

    void Transform(FileServer server, IMediaFolder root);
  }
}
