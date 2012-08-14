using System.Linq;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class PlainView : IView
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
      var root = Root as IFileServerFolder;
      MushFolder(Server, root, root);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseItem(i as IFileServerMediaItem);
      }
    }

    private void MushFolder(FileServer server, IFileServerFolder root, IFileServerFolder folder)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        MushFolder(server, root, f as IFileServerFolder);
      }
      foreach (var c in folder.ChildItems.ToList()) {
        root.AdoptItem(c as IFileServerMediaItem);
      }
    }
  }
}
