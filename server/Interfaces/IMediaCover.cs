
namespace NMaier.sdlna.Server
{
  public interface IMediaCover : IMediaResource
  {

    IMediaCoverResource Cover { get; }
  }
}
