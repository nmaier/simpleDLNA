
namespace NMaier.sdlna.Server.Metadata
{
  public interface IMetaAudioItem : IMetaInfo, IMetaDescription, IMetaDuration, IMetaGenre {
    string MetaArtist { get; }
    string MetaPerformer { get; }
    string MetaAlbum { get; }

  }
}
