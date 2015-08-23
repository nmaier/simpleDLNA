using NMaier.SimpleDlna.Server.Metadata;
using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class ByDateView : BaseView
  {
    public override string Description
    {
      get
      {
        return "Reorganizes files into folders by date";
      }
    }

    public override string Name
    {
      get
      {
        return "bydate";
      }
    }

    private static void SortFolder(VirtualFolder folder, TitlesFolder titles)
    {
      folder.AllItems.GroupBy(
        r => (r is IMetaInfo) ? ((r as IMetaInfo).InfoDate.ToString("yyyy-MMM")) : "Unknown",
        r => r,
        (k, g) => new { Key = k, Lst = g.ToList() }
      )
      .ToList()
      .ForEach(i => {
        var tf = titles.GetFolder(i.Key);
        i.Lst.ForEach(r => {
          tf.AddResource(r);
          folder.RemoveResource(r);
        });
     });
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
