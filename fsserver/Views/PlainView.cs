using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
{
  internal sealed class PlainView : IView
  {

    public string Description
    {
      get { return "Mushes all files together into the root folder"; }
    }

    public string Name
    {
      get { return "plain"; }
    }




    public IMediaFolder Transform(FileServer Server, IMediaFolder Root)
    {
      var rv = new VirtualFolder(Server, null, "0");
      EatAll(rv, Root);
      return rv;
    }

    private void EatAll(BaseFolder root, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        EatAll(root, f);
      }
      foreach (var c in folder.ChildItems.ToList()) {
        root.AddFile(c as Files.BaseFile);
      }
    }
  }
}
