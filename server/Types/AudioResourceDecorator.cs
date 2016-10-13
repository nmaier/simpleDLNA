using System;

namespace NMaier.SimpleDlna.Server
{
  internal class AudioResourceDecorator
    : MediaResourceDecorator<IMediaAudioResource>
  {
    public AudioResourceDecorator(IMediaAudioResource resource)
      : base(resource)
    {
    }

    public virtual string MetaAlbum => Resource.MetaAlbum;

    public virtual string MetaArtist => Resource.MetaArtist;

    public virtual string MetaDescription => Resource.MetaDescription;

    public virtual TimeSpan? MetaDuration => Resource.MetaDuration;

    public virtual string MetaGenre => Resource.MetaGenre;

    public virtual string MetaPerformer => Resource.MetaPerformer;

    public virtual int? MetaTrack => Resource.MetaTrack;
  }
}
