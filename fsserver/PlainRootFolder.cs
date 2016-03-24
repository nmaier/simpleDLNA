using NMaier.SimpleDlna.Server;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class PlainRootFolder : PlainFolder
  {
    internal PlainRootFolder(FileServer server,  DirectoryInfo di)
      : base(server, null, di)
    {
      Id = Identifiers.GeneralRoot;
    }
  }
}
