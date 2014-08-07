using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Thumbnails;
using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.FileMediaServer
{
  [Serializable]
  internal sealed class Cover : Logging, IMediaCoverResource, IMetaInfo, ISerializable
  {
    private byte[] bytes;

    private readonly FileInfo file;

    private int height = 240;

    private static readonly ThumbnailMaker thumber =
      new ThumbnailMaker();

    private int width = 240;

    private Cover(SerializationInfo info, StreamingContext ctx)
    {
      bytes = info.GetValue("b", typeof(byte[])) as byte[];
      width = info.GetInt32("w");
      height = info.GetInt32("h");
      var di = ctx.Context as DeserializeInfo;
      if (di != null) {
        file = di.Info;
      }
    }

    internal Cover(FileInfo aFile, Stream aStream)
    {
      var thumb = thumber.GetThumbnail(
        aFile.FullName,
        DlnaMediaTypes.Image,
        aStream,
        width,
        height
        );
      bytes = thumb.GetData();
      height = thumb.Height;
      width = thumb.Width;
    }

    public Cover(FileInfo aFile)
    {
      file = aFile;
    }

    internal event EventHandler OnCoverLazyLoaded;

    private byte[] Bytes
    {
      get
      {
        var rv = ForceLoad();
        if (rv == null || rv.Length == 0) {
          throw new NotSupportedException();
        }
        return rv;
      }
    }

    IMediaCoverResource IMediaCover.Cover
    {
      get
      {
        return this;
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

    public DateTime InfoDate
    {
      get
      {
        if (file != null) {
          return file.LastWriteTimeUtc;
        }
        return DateTime.Now;
      }
    }

    public long? InfoSize
    {
      get
      {
        try {
          var b = Bytes;
          if (b != null) {
            return b.Length;
          }
          return null;
        }
        catch (NotSupportedException) {
          return null;
        }
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
        return "JPEG_TN";
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
        return DlnaMime.ImageJPEG;
      }
    }

    internal byte[] ForceLoad()
    {
      try {
        if (bytes == null) {
          var thumb = thumber.GetThumbnail(
            file,
            width,
            height
            );
          bytes = thumb.GetData();
          height = thumb.Height;
          width = thumb.Width;
        }
      }
      catch (NotSupportedException ex) {
        Debug("Failed to load thumb for " + file.FullName, ex);
      }
      catch (Exception ex) {
        Warn("Failed to load thumb for " + file.FullName, ex);
        return null;
      }
      if (bytes == null) {
        bytes = new byte[0];
      }
      if (OnCoverLazyLoaded != null) {
        OnCoverLazyLoaded(this, null);
      }
      return bytes;
    }

    public int CompareTo(IMediaItem other)
    {
      throw new NotSupportedException();
    }

    public Stream CreateContentStream()
    {
      return new MemoryStream(Bytes);
    }

    public bool Equals(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      if (info == null) {
        throw new ArgumentNullException("info");
      }
      if (bytes == null) {
        throw new NotSupportedException("No cover loaded");
      }
      info.AddValue("b", bytes);
      info.AddValue("w", width);
      info.AddValue("h", height);
    }

    public string ToComparableTitle()
    {
      throw new NotImplementedException();
    }
  }
}
