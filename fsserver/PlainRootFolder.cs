using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class PlainRootFolder : PlainFolder
  {
    internal PlainRootFolder(FileServer server, DirectoryInfo di)
      : base(server, null, di)
    {
      Id = Identifiers.GENERAL_ROOT;
    }
  }
}
