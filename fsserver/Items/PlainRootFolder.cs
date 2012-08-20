using System.IO;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  internal class PlainRootFolder : PlainFolder
  {

    public PlainRootFolder(FileServer server, MediaTypes types, DirectoryInfo di)
      : base(server, types, null, di)
    {
      ID = "0";
    }
  }
}
