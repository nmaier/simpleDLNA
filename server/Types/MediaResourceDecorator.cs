using System;
using System.IO;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.Server
{
  internal class MediaResourceDecorator<T> : IMediaResource, IMetaInfo
    where T : IMediaResource, IMetaInfo
  {
    protected T Resource;

    public MediaResourceDecorator(T resource)
    {
      Resource = resource;
    }

    public virtual IMediaCoverResource Cover => Resource.Cover;

    public string Id
    {
      get { return Resource.Id; }
      set { Resource.Id = value; }
    }

    public virtual DlnaMediaTypes MediaType => Resource.MediaType;

    public string Path => Resource.Path;

    public virtual string PN => Resource.PN;

    public virtual IHeaders Properties => Resource.Properties;

    public virtual string Title => Resource.Title;

    public DlnaMime Type => Resource.Type;

    public virtual int CompareTo(IMediaItem other)
    {
      return Resource.CompareTo(other);
    }

    public virtual Stream CreateContentStream()
    {
      return Resource.CreateContentStream();
    }

    public bool Equals(IMediaItem other)
    {
      return Resource.Equals(other);
    }

    public string ToComparableTitle()
    {
      return Resource.ToComparableTitle();
    }

    public DateTime InfoDate => Resource.InfoDate;

    public long? InfoSize => Resource.InfoSize;
  }
}
