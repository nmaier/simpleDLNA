
namespace NMaier.sdlna.Server
{
  public interface IMediaServerMount
  {

    void RegisterMediaServer(IMediaServer aMediaServer);

    void UnregisterMediaServer(IMediaServer aMediaServer);
  }
}
