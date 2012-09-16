using System;
using System.IO;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;
using NMaier.sdlna.Util;

namespace NMaier.sdlna.FileMediaServer.Files
{
  internal class BaseFile : Logging, IMediaResource, IFileServerMediaItem, IMediaCover, IMetaInfo
  {

    private WeakReference _cover = new WeakReference(null);
    private static readonly LRUCache<string, Cover> coverCache = new LRUCache<string, Cover>(500);
    private DateTime? lastModified = null;
    private long? length = null;
    private readonly string title;



    protected BaseFile(BaseFolder aParent, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
    {
      Parent = aParent;
      Item = aFile;

      length = Item.Length;
      lastModified = Item.LastWriteTimeUtc;

      Type = aType;
      MediaType = aMediaType;

      title = System.IO.Path.GetFileNameWithoutExtension(Item.Name);
      if (string.IsNullOrEmpty(title)) {
        title = Item.Name;
      }
      try {
        title = Uri.UnescapeDataString(title);
      }
      catch (Exception) { }
      if (!title.Contains(" ")) {
        foreach (var c in new char[] { '_', '+', '.' }) {
          title = title.Replace(c, ' ');
        }
      }
    }



    public Stream Content
    {
      get
      {
        return new FileStream(
          Item.FullName,
          FileMode.Open,
          FileAccess.Read,
          FileShare.ReadWrite | FileShare.Delete
          );
      }
    }

    protected Cover cover
    {
      get { return _cover.Target as Cover; }
      set
      {
        if (value != null) {
          coverCache[Item.FullName] = value;
        }
        _cover = new WeakReference(value);
      }
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

    public DateTime Date
    {
      get
      {
        if (lastModified == null) {
          lastModified = Item.LastWriteTimeUtc;
        }
        return lastModified.Value;
      }
    }

    public string ID
    {
      get;
      set;
    }

    IMediaFolder IMediaItem.Parent
    {
      get { return Parent; }
    }

    internal FileInfo Item
    {
      get;
      set;
    }

    public MediaTypes MediaType
    {
      get;
      protected set;
    }

    public BaseFolder Parent
    {
      get;
      set;
    }

    public string Path
    {
      get { return Item.FullName; }
    }

    public string PN
    {
      get { return DlnaMaps.PN[Type]; }
    }

    public virtual IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Title", Title);
        rv.Add("MediaType", MediaType.ToString());
        rv.Add("Type", Type.ToString());
        rv.Add("SizeRaw", Size.ToString());
        rv.Add("Size", Util.Formatting.FormatFileSize(Size.Value));
        rv.Add("Date", Date.ToString());
        rv.Add("DateO", Date.ToString("o"));
        try {
          if (Cover != null) {
            rv.Add("HasCover", "true");
          }
        }
        catch (Exception) { }
        return rv;
      }
    }

    public long? Size
    {
      get
      {
        if (length == null) {
          length = Item.Length;
        }
        return length;
      }
    }

    public virtual string Title
    {
      get { return title; }
    }

    public DlnaTypes Type
    {
      get;
      protected set;
    }




    public virtual int CompareTo(IMediaItem other)
    {
      return Title.ToLower().CompareTo(other.Title.ToLower());
    }

    public void LazyLoadedCover(object sender, EventArgs e)
    {
      Parent.Server.UpdateFileCache(this);
    }

    public void LoadCover()
    {
      if (cover != null) {
        return;
      }
      cover = new Cover(Item);
      cover.OnCoverLazyLoaded += LazyLoadedCover;
      cover.ForceLoad();
      cover = null;
    }

    protected bool LoadCoverFromCache()
    {
      cover = Parent.Server.GetCover(this);
      return cover != null;
    }

    internal static BaseFile GetFile(BaseFolder aParentFolder, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
    {
      switch (aMediaType) {
        case MediaTypes.VIDEO:
          return new VideoFile(aParentFolder, aFile, aType);
        case MediaTypes.AUDIO:
          return new AudioFile(aParentFolder, aFile, aType);
        case MediaTypes.IMAGE:
          return new ImageFile(aParentFolder, aFile, aType);
        default:
          return new BaseFile(aParentFolder, aFile, aType, aMediaType);
      }
    }

    internal Cover MaybeGetCover()
    {
      return cover;
    }
  }
}
