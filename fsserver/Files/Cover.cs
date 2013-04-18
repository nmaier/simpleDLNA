using System;
using System.IO;
using System.Runtime.Serialization;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Thumbnails;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer
{
  [Serializable]
  internal sealed class Cover : Logging, IMediaCoverResource, ISerializable
  {
    private byte[] _bytes;

    private readonly FileInfo file;

    private int height = 240;

    private static readonly ThumbnailMaker thumber = new ThumbnailMaker();

    private int width = 240;


    private Cover(SerializationInfo info, StreamingContext ctx)
    {
      _bytes = info.GetValue("bytes", typeof(byte[])) as byte[];
      width = info.GetInt32("width");
      height = info.GetInt32("height");
    }


    internal Cover(FileInfo aFile, Stream aStream)
    {
      _bytes = thumber.GetThumbnail(aFile.FullName, DlnaMediaTypes.Image, aStream, ref width, ref height);
    }


    public Cover(FileInfo aFile)
    {
      file = aFile;
    }


    internal event EventHandler OnCoverLazyLoaded;


    private byte[] bytes
    {
      get
      {
        if (_bytes == null) {
          ForceLoad();
        }
        if (_bytes.Length == 0) {
          throw new NotSupportedException();
        }
        return _bytes;
      }
    }


    public Stream Content
    {
      get
      {
        return new MemoryStream(bytes);
      }
    }
    public string Id
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }
    public DlnaMediaTypes MediaType
    {
      get
      {
        return DlnaMediaTypes.Image;
      }
    }
    public int? MetaHeight
    {
      get
      {
        return height;
      }
    }
    public int? MetaWidth
    {
      get
      {
        return width;
      }
    }
    public string Path
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    public string PN
    {
      get
      {
        return "DLNA.ORG_PN=JPEG_TN";
      }
    }
    public IHeaders Properties
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    public string Title
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    public DlnaMime Type
    {
      get
      {
        return DlnaMime.JPEG;
      }
    }


    internal void ForceLoad()
    {
      try {
        if (_bytes == null) {
          _bytes = thumber.GetThumbnail(file, ref width, ref height);
        }
      }
      catch (Exception ex) {
        Warn("Failed to load thumb for " + file.FullName, ex);
      }
      if (_bytes == null) {
        _bytes = new byte[0];
      }
      if (OnCoverLazyLoaded != null) {
        OnCoverLazyLoaded(this, null);
      }
    }


    public int CompareTo(IMediaItem other)
    {
      throw new NotSupportedException();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      if (info == null) {
        throw new ArgumentNullException("info");
      }
      info.AddValue("bytes", _bytes);
      info.AddValue("width", width);
      info.AddValue("height", height);
    }
  }
}
