using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  using CoverCache = LeastRecentlyUsedDictionary<string, Cover>;

  internal class BaseFile : Logging, IMediaResource, IMediaCover, IMetaInfo
  {
    private string comparableTitle;

    private DateTime? lastModified = null;

    private long? length = null;

    private readonly FileServer server;

    private readonly string title;

    private WeakReference weakCover = new WeakReference(null);

    private static readonly CoverCache coverCache = new CoverCache(50);

    private static readonly StringComparer comparer =
      new NaturalStringComparer(false);

    protected BaseFile(FileServer server, FileInfo file, DlnaMime type,
                       DlnaMediaTypes mediaType)
    {
      if (server == null) {
        throw new ArgumentNullException("server");
      }
      this.server = server;
      Item = file;

      length = Item.Length;
      lastModified = Item.LastWriteTimeUtc;

      Type = type;
      MediaType = mediaType;

      title = System.IO.Path.GetFileNameWithoutExtension(Item.Name);
      if (string.IsNullOrEmpty(title)) {
        title = Item.Name;
      }
      if (!string.IsNullOrWhiteSpace(title)) {
        try {
          title = Uri.UnescapeDataString(title);
        }
        catch (UriFormatException) {
          // no op
        }
      }
      title = title.StemNameBase();
    }

    protected Cover cover
    {
      get
      {
        return weakCover.Target as Cover;
      }
      set
      {
        if (value != null) {
          coverCache[Item.FullName] = value;
        }
        weakCover = new WeakReference(value);
      }
    }

    protected FileServer Server
    {
      get
      {
        return server;
      }
    }

    internal FileInfo Item
    {
      get;
      set;
    }

    public virtual IMediaCoverResource Cover
    {
      get
      {
        if (cover == null && !LoadCoverFromCache()) {
          cover = new Cover(Item);
          cover.OnCoverLazyLoaded += LazyLoadedCover;
        }
        return cover;
      }
    }

    public string Id
    {
      get;
      set;
    }

    public DateTime InfoDate
    {
      get
      {
        if (!lastModified.HasValue) {
          lastModified = Item.LastWriteTimeUtc;
        }
        return lastModified.Value;
      }
    }

    public long? InfoSize
    {
      get
      {
        if (!length.HasValue) {
          length = Item.Length;
        }
        return length;
      }
    }

    public DlnaMediaTypes MediaType
    {
      get;
      protected set;
    }

    public string Path
    {
      get
      {
        return Item.FullName;
      }
    }

    public string PN
    {
      get
      {
        return DlnaMaps.MainPN[Type];
      }
    }

    public virtual IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Title", Title);
        rv.Add("MediaType", MediaType.ToString());
        rv.Add("Type", Type.ToString());
        if (InfoSize.HasValue) {
          rv.Add("SizeRaw", InfoSize.ToString());
          rv.Add("Size", InfoSize.Value.FormatFileSize());
        }
        rv.Add("Date", InfoDate.ToString());
        rv.Add("DateO", InfoDate.ToString("o"));
        try {
          if (Cover != null) {
            rv.Add("HasCover", "true");
          }
        }
        catch (Exception ex) {
          Debug("Failed to access cover", ex);
        }
        return rv;
      }
    }

    public virtual string Title
    {
      get
      {
        return title;
      }
    }

    public DlnaMime Type
    {
      get;
      protected set;
    }

    protected bool LoadCoverFromCache()
    {
      cover = Server.GetCover(this);
      return cover != null;
    }

    internal static BaseFile GetFile(PlainFolder parentFolder, FileInfo file,
                                     DlnaMime type, DlnaMediaTypes mediaType)
    {
      switch (mediaType) {
        case DlnaMediaTypes.Video:
          return new VideoFile(parentFolder.Server, file, type);
        case DlnaMediaTypes.Audio:
          return new AudioFile(parentFolder.Server, file, type);
        case DlnaMediaTypes.Image:
          return new ImageFile(parentFolder.Server, file, type);
        default:
          return new BaseFile(parentFolder.Server, file, type, mediaType);
      }
    }

    internal Cover MaybeGetCover()
    {
      return cover;
    }

    public virtual int CompareTo(IMediaItem other)
    {
      if (other == null) {
        return 1;
      }
      return comparer.Compare(title, other.Title);
    }

    public Stream CreateContentStream()
    {
      try {
        return FileStreamCache.Get(Item);
      }
      catch (FileNotFoundException ex) {
        Error("Failed to access: " + Item.FullName, ex);
        server.DelayedRescan(WatcherChangeTypes.Deleted);
        throw;
      }
      catch (UnauthorizedAccessException ex) {
        Error("Failed to access: " + Item.FullName, ex);
        server.DelayedRescan(WatcherChangeTypes.Changed);
        throw;
      }
      catch (IOException ex) {
        Error("Failed to access: " + Item.FullName, ex);
        server.DelayedRescan(WatcherChangeTypes.Changed);
        throw;
      }
    }

    public bool Equals(IMediaItem other)
    {
      if (other == null) {
        return false;
      }
      return comparer.Equals(title, other.Title);
    }

    public void LazyLoadedCover(object sender, EventArgs e)
    {
      Server.UpdateFileCache(this);
    }

    public virtual void LoadCover()
    {
      if (cover != null) {
        return;
      }
      cover = new Cover(Item);
      cover.OnCoverLazyLoaded += LazyLoadedCover;
      cover.ForceLoad();
      cover = null;
    }

    public string ToComparableTitle()
    {
      if (comparableTitle == null) {
        comparableTitle = Title.StemCompareBase();
      }
      return comparableTitle;
    }
  }
}
