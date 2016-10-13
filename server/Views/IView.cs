using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  public interface IView : IRepositoryItem
  {
    IMediaFolder Transform(IMediaFolder oldRoot);
  }
}
