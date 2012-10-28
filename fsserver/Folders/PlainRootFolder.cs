using System.IO;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  internal class PlainRootFolder : PlainFolder
  {

    public PlainRootFolder(string aID, FileServer server, MediaTypes types, DirectoryInfo di)
      : base(server, types, null, di)
    {
      ID = aID;
    }
  }
}
