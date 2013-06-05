using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class PlainView : BaseView
  {
    public override string Description
    {
      get
      {
        return "Mushes all files together into the root folder";
      }
    }
    public override string Name
    {
      get
      {
        return "plain";
      }
    }


    private static void EatAll(IMediaFolder root, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        EatAll(root, f);
      }
      foreach (var c in folder.ChildItems.ToList()) {
        root.AddResource(c);
      }
    }


    public override IMediaFolder Transform(IMediaFolder Root)
    {
      var rv = new VirtualFolder(null, Root.Title, Root.Id);
      EatAll(rv, Root);
      return rv;
    }
  }
}
