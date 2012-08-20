using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Views
{
  public interface IView : IRepositoryItem
  {
    void Transform(FileServer Server, IMediaFolder Root);
  }
}
