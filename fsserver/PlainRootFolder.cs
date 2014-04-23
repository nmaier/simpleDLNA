using NMaier.SimpleDlna.Server;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal class PlainRootFolder : PlainFolder
  {
    public PlainRootFolder(FileServer server, DlnaMediaTypes types, DirectoryInfo di)
      : base(server, types, null, di)
    {
      Id = Identifiers.GeneralRoot;
    }
  }
}
