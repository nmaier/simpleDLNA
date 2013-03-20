namespace NMaier.SimpleDlna.Server
{
  public interface IMediaCover : IMediaResource
  {
    IMediaCoverResource Cover { get; }
  }
}
