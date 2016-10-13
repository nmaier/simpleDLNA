using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class FlattenView : BaseView
  {
    public override string Description => "Removes empty intermediate folders and flattens folders with only few files";

    public override string Name => "flatten";

    private static bool TransformInternal(VirtualFolder root,
      VirtualFolder current)
    {
      foreach (var f in current.ChildFolders.ToList()) {
        var vf = f as VirtualFolder;
        if (TransformInternal(root, vf)) {
          current.ReleaseFolder(vf);
        }
      }

      if (current == root || current.ChildItems.Count() > 3) {
        return false;
      }
      var newParent = (VirtualFolder)current.Parent;
      foreach (var c in current.ChildItems.ToList()) {
        current.RemoveResource(c);
        newParent.AddResource(c);
      }

      if (current.ChildCount != 0) {
        MergeFolders(current, newParent);
        foreach (var f in current.ChildFolders.ToList()) {
          newParent.AdoptFolder(f);
        }
        foreach (var f in current.ChildItems.ToList()) {
          current.RemoveResource(f);
          newParent.AddResource(f);
        }
      }
      return true;
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      var r = new VirtualClonedFolder(oldRoot);
      var cross = from f in r.ChildFolders
                  from t in r.ChildFolders
                  where f != t
                  orderby f.Title, t.Title
                  select new
                  {
                    f = f as VirtualFolder,
                    t = t as VirtualFolder
                  };
      foreach (var c in cross) {
        MergeFolders(c.f, c.t);
      }

      TransformInternal(r, r);
      MergeFolders(r, r);
      return r;
    }
  }
}
