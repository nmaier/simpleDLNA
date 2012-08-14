using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  public interface IView : IRepositoryItem
  {
    void Transform(FileServer Server, IMediaFolder Root);
  }
}
