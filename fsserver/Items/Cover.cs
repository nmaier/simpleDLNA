using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Thumbnails;

namespace NMaier.sdlna.FileMediaServer
{
  internal class Cover : Logging, IMediaResource
  {

    private byte[] _bytes;
    private readonly FileInfo file;
    private static readonly Thumbnailer thumber = new Thumbnailer();



    public Cover(FileInfo aFile)
    {
      file = aFile;
    }



    private byte[] bytes
    {
      get
      {
        if (_bytes == null) {
          _bytes = thumber.GetThumbnail(file, 240, 240);
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