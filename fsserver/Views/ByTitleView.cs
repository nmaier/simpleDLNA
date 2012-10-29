using System.Linq;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
{
  internal sealed class ByTitleView : IView
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




    public IMediaFolder Transform(FileServer Server, IMediaFolder Root)
    {
      var root = new VirtualClonedFolder(Root as BaseFolder);
      var titles = new TitlesFolder(Server, root);
      SortFolder(Server, root, titles);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseFolder(i as BaseFolder);
      }
      foreach (var i in titles.ChildFolders.ToList()) {
        root.AdoptFolder(i as BaseFolder);
      }
      return root;
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
        var file = c as Files.BaseFile;
        titles.GetFolder(pre).AddFile(file);
        folder.RemoveFile(file);
      }
    }




    private class TitlesFolder : KeyedVirtualFolder<VirtualFolder>
    {
      public TitlesFolder(FileServer aServer, BaseFolder aParent) : base(aServer, aParent, "titles") { }
    }
  }
}
