using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Views
{
  internal sealed class ByTitleView : IView
  {
    private class TitlesFolder : KeyedVirtualFolder<VirtualFolder>
    {
      public TitlesFolder(FileServer aServer, BaseFolder aParent) : base(aServer, aParent, "titles") { }
    }


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
      var root = Root as BaseFolder;
      var titles = new TitlesFolder(Server, root);
      SortFolder(Server, root, titles);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseItem(i as IFileServerMediaItem);
      }
      foreach (var i in titles.ChildFolders.ToList()) {
        root.AdoptItem(i as BaseFolder);
      }
    }

    private void SortFolder(FileServer server, BaseFolder folder, TitlesFolder titles)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(server, f as BaseFolder, titles);
      }

      foreach (var c in folder.ChildItems.ToList()) {
        var pre = regClean.Replace(c.Title, "");
        if (string.IsNullOrEmpty(pre)) {
          pre = "Unnamed";
        }
        pre = pre.First().ToString().ToUpper();
        titles.GetFolder(pre).AdoptItem(c as IFileServerMediaItem);
      }
    }
  }
}
