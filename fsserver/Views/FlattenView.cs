using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
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


    private static void MergeFolders(BaseFolder aFrom, BaseFolder aTo)
    {
      var merges = from f in aFrom.ChildFolders
                   join t in aTo.ChildFolders on f.Title equals t.Title
                   where f != t
                   select new { f = f as BaseFolder, t = t as BaseFolder };
      foreach (var m in merges.ToList()) {
        MergeFolders(m.f, m.t);
        foreach (var c in m.f.ChildFolders.ToList()) {
          m.t.AdoptFolder(c as BaseFolder);
        }
        foreach (var c in m.f.ChildItems.ToList()) {
          var file = c as Files.BaseFile;
          m.t.AddFile(file);
          m.f.RemoveFile(file);
        }
        (m.f.Parent as BaseFolder).ReleaseFolder(m.f);
      }
    }

    private static bool TransformInternal(BaseFolder root, BaseFolder current)
    {
      foreach (var f in current.ChildFolders.ToList()) {
        var bf = f as BaseFolder;
        if (TransformInternal(root, bf)) {
          current.ReleaseFolder(bf);
        }
      }

      if (current == root || current.ChildItems.Count() > 3) {
        return false;
      }
      var newParent = current.Parent as BaseFolder;
      foreach (var c in current.ChildItems.ToList()) {
        newParent.AddFile(c as Files.BaseFile);
      }

      if (current.ChildCount != 0) {
        MergeFolders(current, newParent);
        foreach (var f in current.ChildFolders.ToList()) {
          newParent.AdoptFolder(f as BaseFolder);
        }
        foreach (var f in current.ChildItems.ToList()) {
          var file = f as Files.BaseFile;
          newParent.AddFile(file);
          current.RemoveFile(file);
        }
      }
      return true;
    }


    public IMediaFolder Transform(FileServer Server, IMediaFolder Root)
    {
      var r = new VirtualClonedFolder(Root as BaseFolder);
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
      return r;
    }
  }
}
