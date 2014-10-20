using NMaier.SimpleDlna.Server;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class PlainRootFolder : PlainFolder
  {
    internal PlainRootFolder(FileServer server, DlnaMediaTypes types,
                             DirectoryInfo di)
      : base(server, types, null, di)
    {
      Id = Identifiers.GeneralRoot;
    }
  }
}
