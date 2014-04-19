using System.Linq;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class ByTitleView : BaseView
  {
    public override string Description
    {
      get
      {
        return "Reorganizes files into folders by title";
      }
    }

    public override string Name
    {
      get
      {
        return "bytitle";
      }
    }

    private static void SortFolder(VirtualFolder folder, TitlesFolder titles)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(f as VirtualFolder, titles);
      }

      foreach (var c in folder.ChildItems.ToList()) {
        var pre = c.Title.StemCompareBase();
        if (string.IsNullOrEmpty(pre)) {
          pre = "Unnamed";
        }
        pre = pre.First().ToString().ToUpper();
        titles.GetFolder(pre).AddResource(c);
        folder.RemoveResource(c);
      }
    }

    public override IMediaFolder Transform(IMediaFolder Root)
    {
      var root = new VirtualClonedFolder(Root);
      var titles = new TitlesFolder(root);
      SortFolder(root, titles);
      foreach (var i in root.ChildFolders.ToList()) {
        root.ReleaseFolder(i);
      }
      foreach (var i in titles.ChildFolders.ToList()) {
        root.AdoptFolder(i);
      }
      return root;
    }

    private class TitlesFolder : KeyedVirtualFolder<VirtualFolder>
    {
      public TitlesFolder(IMediaFolder aParent)
        : base(aParent, "titles")
      {
      }
    }
  }
}
