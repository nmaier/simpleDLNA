using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal static class ByteVectorExtend
  {
    internal static Stream ToStream(this TagLib.ByteVector aVector)
    {
      return new MemoryStream(aVector.Data);
    }
  }
}
