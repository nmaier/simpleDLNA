using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Thumbnails;

namespace NMaier.sdlna.FileMediaServer
{
  internal class Cover : IMediaCoverResource
  {

    private byte[] _bytes;
    private readonly FileInfo file;
    private int height = 240;
    internal static readonly Thumbnailer thumber = new Thumbnailer();
    private int width = 240;



    internal Cover(byte[] aBytes, int aWidth, int aHeight)
    {
      _bytes = aBytes;
      width = aWidth;
      height = aHeight;
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




    public int CompareTo(IMediaItem other)
    {
      throw new NotImplementedException();
    }
  }
}