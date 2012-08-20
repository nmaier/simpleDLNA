using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer
{
  internal class File : Logging, IMediaResource, IFileServerResource, IMediaCover, IMetaInfo
  {
    private readonly string title;

    protected File(IFileServerFolder aParent, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
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

    internal FileInfo Item
    {
      get;
      set;
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

    public MediaTypes MediaType
    {
      get;
      protected set;
    }

    public IFileServerFolder Parent
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




    public int CompareTo(IMediaItem other)
    {
      return Title.CompareTo(other.Title);
    }

    internal static File GetFile(IFileServerFolder aParentFolder, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
    {
      switch (aMediaType) {
        case MediaTypes.VIDEO:
          return new VideoFile(aParentFolder, aFile, aType);
        case MediaTypes.AUDIO:
          return new AudioFile(aParentFolder, aFile, aType);
        case MediaTypes.IMAGE:
          return new ImageFile(aParentFolder, aFile, aType);
        default:
          return new File(aParentFolder, aFile, aType, aMediaType);
      }
    }
  }
}
