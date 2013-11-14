using System;

namespace NMaier.SimpleDlna.Server
{
  internal class AudioResourceDecorator : MediaResourceDecorator<IMediaAudioResource>
  {
    public AudioResourceDecorator(IMediaAudioResource resource)
      : base(resource)
    {
    }


    virtual public IMediaCoverResource Cover
    {
      get
      {
        return resource.Cover;
      }
    }
    public virtual string MetaAlbum
    {
      get
      {
        return resource.MetaAlbum;
      }
    }
    public virtual string MetaArtist
    {
      get
      {
        return resource.MetaArtist;
      }
    }
    public virtual string MetaDescription
    {
      get
      {
        return resource.MetaDescription;
      }
    }
    public virtual TimeSpan? MetaDuration
    {
      get
      {
        return resource.MetaDuration;
      }
    }
    public virtual string MetaGenre
    {
      get
      {
        return resource.MetaGenre;
      }
    }
    public virtual string MetaPerformer
    {
      get
      {
        return resource.MetaPerformer;
      }
    }
    public virtual int? MetaTrack
    {
      get
      {
        return resource.MetaTrack;
      }
    }
  }
}
