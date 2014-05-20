using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal abstract class FilteringView : BaseView
  {
    private void ProcessFolder(IMediaFolder root)
    {
      foreach (var f in root.ChildFolders) {
        ProcessFolder(f);
      }
      foreach (var f in root.ChildItems.ToList()) {
        if (DoFilter(f)) {
          continue;
        }
        root.RemoveResource(f);
      }
    }

    protected abstract bool DoFilter(IMediaResource res);

    public override IMediaFolder Transform(IMediaFolder root)
    {
      root = new VirtualClonedFolder(root);
      ProcessFolder(root);
      return root;
    }
  }
}
