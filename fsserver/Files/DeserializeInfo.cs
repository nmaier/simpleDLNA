using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class DeserializeInfo
  {
    public FileInfo Info;

    public FileServer Server;

    public DlnaMime Type;

    public DeserializeInfo(FileServer server, FileInfo info, DlnaMime type)
    {
      Server = server;
      Info = info;
      Type = type;
    }
  }
}
