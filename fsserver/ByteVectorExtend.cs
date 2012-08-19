using System.IO;

namespace NMaier.sdlna.FileMediaServer
{
  internal static class ByteVectorExtend
  {
    internal static Stream ToStream(this TagLib.ByteVector aVector)
    {
      return new MemoryStream(aVector.Data);
    }
  }
}
