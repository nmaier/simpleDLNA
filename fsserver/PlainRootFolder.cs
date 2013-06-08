using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal class PlainRootFolder : PlainFolder
  {
    public PlainRootFolder(FileServer server, DlnaMediaTypes types, DirectoryInfo di)
      : base(server, types, null, di)
    {
      Id = Identifiers.KeyRoot;
    }
  }
}
