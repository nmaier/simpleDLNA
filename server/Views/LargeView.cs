using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class LargeView : BaseView
  {
    private long minSize = 300 * 1024 * 1024;

    public override string Description
    {
      get
      {
        return "Show only large files";
      }
    }

    public override string Name
    {
      get
      {
        return "large";
      }
    }

    private void ProcessFolder(IMediaFolder root)
    {
      foreach (var f in root.ChildFolders) {
        ProcessFolder(f);
      }
      foreach (var f in root.ChildItems.ToList()) {
        var i = f as IMetaInfo;
        if (i == null) {
          continue;
        }
        if (i.InfoSize.HasValue && i.InfoSize.Value >= minSize) {
          continue;
        }
        root.RemoveResource(f);
      }
    }

    public override void SetParameters(AttributeCollection parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException("parameters");
      }
      base.SetParameters(parameters);

      foreach (var v in parameters.GetValuesForKey("size")) {
        var min = 0L;
        if (long.TryParse(v, out min) && min > 0) {
          minSize = min * 1024 * 1024;
          break;
        }
      }
    }

    public override IMediaFolder Transform(IMediaFolder root)
    {
      root = new VirtualClonedFolder(root);
      ProcessFolder(root);
      return root;
    }
  }
}
