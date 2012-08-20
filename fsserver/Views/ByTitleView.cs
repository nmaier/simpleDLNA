using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class ByTitleView : IView
  {

    private static Regex regClean = new Regex(@"[^\d\w]+", RegexOptions.Compiled);



    public string Description
    {
      get { return "Reorganizes files into folders by title"; }
    }

    public string Name
    {
      get { return "bytitle"; }
    }




    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var root = Root as IFileServerFolder;
      var titles = new Dictionary<string, IFileServerFolder>();
      SortFolder(Server, root, titles);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseItem(i as IFileServerMediaItem);
      }
      foreach (var i in titles.Values) {
        root.AdoptItem(i);
      }
    }

    private void SortFolder(FileServer server, IFileServerFolder folder, IDictionary<string, IFileServerFolder> titles)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(server, f as IFileServerFolder, titles);
      }

      foreach (var c in folder.ChildItems.ToList()) {
        var pre = regClean.Replace(c.Title, "");
        if (string.IsNullOrEmpty(pre)) {
          pre = "Unnamed";
        }
        pre = pre.First().ToString().ToUpper();
        IFileServerFolder dest;
        if (!titles.TryGetValue(pre, out dest)) {
          dest = new VirtualFolder(server, server.Root as IFileServerFolder, pre);
          titles.Add(pre, dest);
          server.RegisterPath(dest);
        }
        dest.AdoptItem(c as IFileServerMediaItem);
      }
    }
  }
}
