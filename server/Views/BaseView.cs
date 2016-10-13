using System.Linq;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal abstract class BaseView : Logging, IView
  {
    public abstract string Description { get; }

    public abstract string Name { get; }

    public abstract IMediaFolder Transform(IMediaFolder oldRoot);

    public override string ToString()
    {
      return $"{Name} - {Description}";
    }

    protected static void MergeFolders(VirtualFolder aFrom, VirtualFolder aTo)
    {
      var merges = from f in aFrom.ChildFolders
                   join t in aTo.ChildFolders on f.Title.ToUpper() equals t.Title.ToUpper()
                   where f != t
                   select new
                   {
                     f = f as VirtualFolder,
                     t = t as VirtualFolder
                   };
      foreach (var m in merges.ToList()) {
        MergeFolders(m.f, m.t);
        foreach (var c in m.f.ChildFolders.ToList()) {
          m.t.AdoptFolder(c);
        }
        foreach (var c in m.f.ChildItems.ToList()) {
          m.t.AddResource(c);
          m.f.RemoveResource(c);
        }
        if (aFrom != aTo) {
          ((VirtualFolder)m.f.Parent).ReleaseFolder(m.f);
        }
      }
    }
  }
}
