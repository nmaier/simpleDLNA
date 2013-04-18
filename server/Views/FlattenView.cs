using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class FlattenView : IView
  {
    public string Description
    {
      get
      {
        return "Removes empty intermediate folders and flattens folders with only few files";
      }
    }
    public string Name
    {
      get
      {
        return "flatten";
      }
    }


    private static void MergeFolders(VirtualFolder aFrom, VirtualFolder aTo)
    {
      var merges = from f in aFrom.ChildFolders
                   join t in aTo.ChildFolders on f.Title equals t.Title
                   where f != t
                   select new { f = f as VirtualFolder, t = t as VirtualFolder };
      foreach (var m in merges.ToList()) {
        MergeFolders(m.f, m.t);
        foreach (var c in m.f.ChildFolders.ToList()) {
          m.t.AdoptFolder(c);
        }
        foreach (var c in m.f.ChildItems.ToList()) {
          m.t.AddResource(c);
          m.f.RemoveResource(c);
        }
        (m.f.Parent as VirtualFolder).ReleaseFolder(m.f);
      }
    }

    private static bool TransformInternal(VirtualFolder root, VirtualFolder current)
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
      var newParent = current.Parent as VirtualFolder;
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


    public IMediaFolder Transform(IMediaFolder Root)
    {
      var r = new VirtualClonedFolder(Root);
      var cross = from f in r.ChildFolders
                  from t in r.ChildFolders
                  where f != t
                  orderby f.Title, t.Title
                  select new { f = f as VirtualFolder, t = t as VirtualFolder };
      foreach (var c in cross) {
        MergeFolders(c.f, c.t);
      }

      TransformInternal(r, r);
      MergeFolders(r, r);
      return r;
    }
  }
}
