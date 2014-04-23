using System;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class TagLibFileAbstraction : TagLib.File.IFileAbstraction
  {
    private readonly FileInfo info;

    public TagLibFileAbstraction(FileInfo info)
    {
      this.info = info;
    }

    public string Name
    {
      get
      {
        return info.FullName;
      }
    }

    public Stream ReadStream
    {
      get
      {
        return info.Open(
          FileMode.Open,
          FileAccess.Read,
          FileShare.ReadWrite
          );
      }
    }

    public Stream WriteStream
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void CloseStream(Stream stream)
    {
      if (stream == null) {
        return;
      }
      stream.Close();
      stream.Dispose();
    }
  }
}
