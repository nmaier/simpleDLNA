using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer
{
  internal class File : Logging, IMediaResource, IFileServerResource, IMediaCover, IMetaInfo
  {

    protected readonly FileInfo file;
    private string id;
    private readonly MediaTypes mediaType;
    private IMediaFolder parent;
    private readonly string title;
    private readonly DlnaTypes type;



    protected File(IMediaFolder aParent, FileInfo aFile, DlnaTypes aType, MediaTypes aMediaType)
    {
      parent = aParent;
      file = aFile;

      type = aType;
      mediaType = aMediaType;

      title = System.IO.Path.GetFileNameWithoutExtension(file.Name);
      if (string.IsNullOrEmpty(title)) {
        title = file.Name;
      }
    }



    public Stream Content
    {
      get
      {
        return new FileStream(
          file.FullName,
          FileMode.Open,
          FileAccess.Read,
          FileShare.ReadWrite | FileShare.Delete
          );
      }
    }

    public virtual IMediaCoverResource Cover
    {
      get { return new Cover(file); }
    }

    public DateTime Date
    {
      get { return file.LastWriteTimeUtc; }
    }

    public string ID
    {
      get { return id; }
      set { id = value; }
    }

    public MediaTypes MediaType
    {
      get { return mediaType; }
    }

    public IMediaFolder Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    public string Path
    {
      get { return file.FullName; }
    }

    public string PN
    {
      get { return DlnaMaps.PN[type]; }
    }

    public long? Size
    {
      get { return file.Length; }
    }

    public virtual string Title
    {
      get { return title; }
    }

    public DlnaTypes Type
    {
      get { return type; }
    }




    public int CompareTo(IMediaItem other)
    {
      return Title.CompareTo(other.Title);
    }

    internal static File GetFile(IMediaFolder aParentFolder, FileInfo aFile)
    {
      var ext = aFile.Extension.ToLower().Substring(1);
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];
      switch (mediaType) {
        case MediaTypes.VIDEO:
          return new VideoFile(aParentFolder, aFile, type);
        case MediaTypes.AUDIO:
          return new AudioFile(aParentFolder, aFile, type);
        case MediaTypes.IMAGE:
          return new ImageFile(aParentFolder, aFile, type);
        default:
          return new File(aParentFolder, aFile, type, mediaType);
      }
    }
  }
}
