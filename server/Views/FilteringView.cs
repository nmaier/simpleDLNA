using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal abstract class FilteringView : BaseView, IFilteredView
  {
    public abstract bool Allowed(IMediaResource item);

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      oldRoot = new VirtualClonedFolder(oldRoot);
      ProcessFolder(oldRoot);
      return oldRoot;
    }

    private void ProcessFolder(IMediaFolder root)
    {
      foreach (var f in root.ChildFolders) {
        ProcessFolder(f);
      }
      foreach (var f in root.ChildItems.ToList()) {
        if (Allowed(f)) {
          continue;
        }
        root.RemoveResource(f);
      }
    }
  }
}
