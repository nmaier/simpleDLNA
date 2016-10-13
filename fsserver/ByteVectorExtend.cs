using System.IO;
using TagLib;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal static class ByteVectorExtend
  {
    internal static Stream ToStream(this ByteVector aVector) => new MemoryStream(aVector.Data);
  }
}
