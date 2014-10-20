namespace NMaier.SimpleDlna.Server.Metadata
{
  public interface IMetaAudioItem
    : IMetaInfo, IMetaDescription, IMetaDuration, IMetaGenre
  {
    string MetaAlbum { get; }

    string MetaArtist { get; }

    string MetaPerformer { get; }

    int? MetaTrack { get; }
  }
}
