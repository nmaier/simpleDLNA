using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Linq;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class LargeView : FilteringView
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

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaInfo;
      if (i == null) {
        return false;
      }
      return i.InfoSize.HasValue && i.InfoSize.Value >= minSize;
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
  }
}
