using System.IO;
using NMaier.sdlna.Server;
using System;

namespace NMaier.sdlna.FileMediaServer
{
  class File : IMediaResource, IFileServerResource, IMediaCover, IMediaItemMetaData
  {

    private readonly FileInfo file;
    private string id;
    private readonly MediaTypes mediaType;
    private IMediaFolder parent;
    private readonly string title;
    private readonly DlnaTypes type;



    public File(IMediaFolder aParent, FileInfo aFile)
    {
      parent = aParent;
      file = aFile;

      var ext = file.Extension.ToLower().Substring(1);
      type = DlnaMaps.Ext2Dlna[ext];
      mediaType = DlnaMaps.Ext2Media[ext];

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

    public IMediaResource Cover
    {
      get { return new Cover(file); }
    }

    public string ID
    {
      get { return id; }
      set { id = value; }
    }

    public DateTime ItemDate
    {
      get { return file.LastWriteTimeUtc; }
    }

    public long ItemSize
    {
      get { return file.Length; }
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

    public string Title
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
  }
}
