using System;
using System.Globalization;
using System.IO;
using System.Net;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer
{
  using CoverCache = LeastRecentlyUsedDictionary<string, Cover>;

  internal class BaseFile : Logging, IMediaResource, IMetaInfo
  {
    private static readonly CoverCache coverCache = new CoverCache(120);

    private static readonly StringComparer comparer =
      new NaturalStringComparer(false);

    private readonly string title;
    private string comparableTitle;

    private DateTime? lastModified;

    private long? length;

    private WeakReference weakCover = new WeakReference(null);

    protected BaseFile(FileServer server, FileInfo file, DlnaMime type,
      DlnaMediaTypes mediaType)
    {
      if (server == null) {
        throw new ArgumentNullException(nameof(server));
      }
      Server = server;
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

    protected Cover CachedCover
    {
      get { return weakCover.Target as Cover; }
      set {
        if (value != null) {
          using (coverCache.AddAndPop(Item.FullName, value)) {
          }
        }
        weakCover = new WeakReference(value);
      }
    }

    protected FileServer Server { get; }

    internal FileInfo Item { get; set; }

    public virtual IMediaCoverResource Cover
    {
      get {
        if (CachedCover != null || LoadCoverFromCache()) {
          return CachedCover;
        }
        CachedCover = new Cover(Item);
        CachedCover.OnCoverLazyLoaded += LazyLoadedCover;
        return CachedCover;
      }
    }

    public string Id { get; set; }

    public DlnaMediaTypes MediaType { get; protected set; }

    public string Path => Item.FullName;

    public string PN => DlnaMaps.MainPN[Type];

    public virtual IHeaders Properties
    {
      get {
        var rv = new RawHeaders {{"Title", Title}, {"MediaType", MediaType.ToString()}, {"Type", Type.ToString()}};
        if (InfoSize.HasValue) {
          rv.Add("SizeRaw", InfoSize.ToString());
          rv.Add("Size", InfoSize.Value.FormatFileSize());
        }
        rv.Add("Date", InfoDate.ToString(CultureInfo.InvariantCulture));
        rv.Add("DateO", InfoDate.ToString("o"));
        try {
          if (Cover != null) {
            rv.Add("HasCover", "true");
          }
        }
        catch (Exception ex) {
          Debug("Failed to access CachedCover", ex);
        }
        return rv;
      }
    }

    public virtual string Title => title;

    public DlnaMime Type { get; protected set; }

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
        Server.DelayedRescan(WatcherChangeTypes.Deleted);
        throw;
      }
      catch (UnauthorizedAccessException ex) {
        Error("Failed to access: " + Item.FullName, ex);
        Server.DelayedRescan(WatcherChangeTypes.Changed);
        throw;
      }
      catch (IOException ex) {
        Error("Failed to access: " + Item.FullName, ex);
        Server.DelayedRescan(WatcherChangeTypes.Changed);
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

    public string ToComparableTitle()
    {
      return comparableTitle ?? (comparableTitle = Title.StemCompareBase());
    }

    public DateTime InfoDate
    {
      get {
        if (!lastModified.HasValue) {
          lastModified = Item.LastWriteTimeUtc;
        }
        return lastModified.Value;
      }
    }

    public long? InfoSize
    {
      get {
        if (!length.HasValue) {
          length = Item.Length;
        }
        return length;
      }
    }

    protected bool LoadCoverFromCache()
    {
      CachedCover = Server.GetCover(this);
      return CachedCover != null;
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
      return CachedCover;
    }

    public void LazyLoadedCover(object sender, EventArgs e)
    {
      Server.UpdateFileCache(this);
    }

    public virtual void LoadCover()
    {
      if (CachedCover != null) {
        return;
      }
      CachedCover = new Cover(Item);
      CachedCover.OnCoverLazyLoaded += LazyLoadedCover;
      CachedCover.ForceLoad();
      CachedCover = null;
    }
  }
}
