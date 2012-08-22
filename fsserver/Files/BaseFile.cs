using System;
using System.IO;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer.Files
{
  internal class BaseFile : Logging, IMediaResource, IFileServerMediaItem, IMediaCover, IMetaInfo
  {

    private readonly string title;



    protected BaseFile(BaseFolder aParent, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
    {
      Parent = aParent;
      Item = aFile;

      Type = aType;
      MediaType = aMediaType;

      title = System.IO.Path.GetFileNameWithoutExtension(Item.Name);
      if (string.IsNullOrEmpty(title)) {
        title = Item.Name;
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

    public virtual IMediaCoverResource Cover
    {
      get { return new Cover(Item); }
    }

    public DateTime Date
    {
      get { return Item.LastWriteTimeUtc; }
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
      get { return Item.Length; }
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
  }
}
