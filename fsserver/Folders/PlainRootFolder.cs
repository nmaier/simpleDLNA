using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class PlainRootFolder : PlainFolder
  {

    public PlainRootFolder(string aID, FileServer server, MediaTypes types, DirectoryInfo di)
      : base(server, types, null, di)
    {
      Id = aID;
    }
  }
}
