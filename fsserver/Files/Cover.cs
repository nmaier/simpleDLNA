using System;
using System.IO;
using System.Runtime.Serialization;
using NMaier.sdlna.Server;
using NMaier.sdlna.Thumbnails;

namespace NMaier.sdlna.FileMediaServer.Files
{
  [Serializable]
  internal sealed class Cover : IMediaCoverResource, ISerializable
  {

    private byte[] _bytes;
    private readonly FileInfo file;
    private int height = 240;
    private static readonly Thumbnailer thumber = new Thumbnailer();
    private int width = 240;



    internal Cover(FileInfo aFile, Stream aStream)
    {
      _bytes = thumber.GetThumbnail(aFile.FullName, MediaTypes.IMAGE, aStream, ref width, ref height);
    }

    private Cover(SerializationInfo info, StreamingContext ctx)
    {
      _bytes = info.GetValue("bytes", typeof(byte[])) as byte[];
      width = info.GetInt32("width");
      height = info.GetInt32("height");
    }

    public Cover(FileInfo aFile)
    {
      file = aFile;
    }



    private byte[] bytes
    {
      get
      {
        if (_bytes == null) {
          _bytes = thumber.GetThumbnail(file, ref width, ref height);
          if (_bytes == null) {
            _bytes = new byte[0];
          }
          if (OnCoverLazyLoaded != null) {
            OnCoverLazyLoaded(this, null);
          }
        }
        if (_bytes.Length == 0) {
          throw new NotSupportedException();
        }
        return _bytes;
      }
    }

    public Stream Content
    {
      get { return new MemoryStream(bytes); }
    }

    public string ID
    {
      get { throw new NotImplementedException(); }
    }

    public MediaTypes MediaType
    {
      get { return MediaTypes.IMAGE; }
    }

    public uint? MetaHeight
    {
      get { return (uint)height; }
    }

    public uint? MetaWidth
    {
      get { return (uint)width; }
    }

    public IMediaFolder Parent
    {
      get { throw new NotImplementedException(); }
    }

    public string PN
    {
      get { return "DLNA.ORG_PN=JPEG_TN"; }
    }

    public IHeaders Properties
    {
      get { throw new NotImplementedException(); }
    }

    public long Size
    {
      get { return bytes.Length; }
    }

    public string Title
    {
      get { throw new NotImplementedException(); }
    }

    public DlnaTypes Type
    {
      get { return DlnaTypes.JPEG; }
    }




    internal event EventHandler OnCoverLazyLoaded;




    public int CompareTo(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      info.AddValue("bytes", _bytes);
      info.AddValue("width", width);
      info.AddValue("height", height);
    }
  }
}