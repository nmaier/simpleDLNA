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




    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var root = Root as BaseFolder;
      MushFolder(Server, root, root);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseItem(i as IFileServerMediaItem);
      }
    }

    private void MushFolder(FileServer server, BaseFolder root, BaseFolder folder)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        MushFolder(server, root, f as BaseFolder);
      }
      foreach (var c in folder.ChildItems.ToList()) {
        root.AdoptItem(c as IFileServerMediaItem);
      }
    }
  }
}
