using System;
using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class PlainView : BaseView
  {
    public override string Description => "Mushes all files together into the root folder";

    public override string Name => "plain";

    private static void EatAll(IMediaFolder root, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        EatAll(root, f);
      }
      foreach (var c in folder.ChildItems.ToList()) {
        root.AddResource(c);
      }
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      if (oldRoot == null) {
        throw new ArgumentNullException(nameof(oldRoot));
      }
      var rv = new VirtualFolder(null, oldRoot.Title, oldRoot.Id);
      EatAll(rv, oldRoot);
      return rv;
    }
  }
}
