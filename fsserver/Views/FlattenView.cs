using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
{
  internal sealed class FlattenView : IView
  {

    public string Description
    {
      get { return "Removes empty intermediate folders and flattens folders with only few files"; }
    }

    public string Name
    {
      get { return "flatten"; }
    }




    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var r = Root as BaseFolder;
      var cross = from f in r.ChildFolders
                  from t in r.ChildFolders
                  where f != t
                  orderby f.Title, t.Title
                  select new { f = f as BaseFolder, t = t as BaseFolder };
      foreach (var c in cross) {
        MergeFolders(c.f, c.t);
      }

      TransformInternal(r, r);
      MergeFolders(r, r);
    }

    private void MergeFolders(BaseFolder aFrom, BaseFolder aTo)
    {
      var merges = from f in aFrom.ChildFolders
                   join t in aTo.ChildFolders on f.Title equals t.Title
                   where f != t
                   select new { f = f as BaseFolder, t = t as BaseFolder };
      foreach (var m in merges.ToList()) {
        MergeFolders(m.f, m.t);
        foreach (var c in m.f.ChildFolders.ToList()) {
          m.t.AdoptItem(c as IFileServerMediaItem);
        }
        foreach (var c in m.f.ChildItems.ToList()) {
          m.t.AdoptItem(c as IFileServerMediaItem);
        }
        (m.f.Parent as BaseFolder).ReleaseItem(m.f);
      }
    }

    bool TransformInternal(BaseFolder root, BaseFolder current)
    {
      foreach (var f in current.ChildFolders.ToList()) {
        if (TransformInternal(root, f as BaseFolder)) {
          current.ReleaseItem(f as IFileServerMediaItem);
        }
      }

      if (current == root || current.ChildItems.Count() > 3) {
        return false;
      }
      var newParent = current.Parent as BaseFolder;
      foreach (var c in current.ChildItems.ToList()) {
        newParent.AdoptItem(c as IFileServerMediaItem);
      }

      if (current.ChildCount != 0) {
        MergeFolders(current, newParent);
        foreach (var f in current.ChildFolders.ToList()) {
          newParent.AdoptItem(f as IFileServerMediaItem);
        }
        foreach (var f in current.ChildItems.ToList()) {
          newParent.AdoptItem(f as IFileServerMediaItem);
        }
      }
      return true;
    }
  }
}
