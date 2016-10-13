using System.IO;
using Microsoft.IO;

namespace NMaier.SimpleDlna.Utilities
{
  public static class StreamManager
  {
    private static readonly RecyclableMemoryStreamManager manager = new RecyclableMemoryStreamManager();

    public static MemoryStream GetStream()
    {
      return manager.GetStream();
    }

    public static MemoryStream GetStream(string tag)
    {
      return manager.GetStream(tag);
    }
  }
}
