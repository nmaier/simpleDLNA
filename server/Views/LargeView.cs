using System;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class LargeView : FilteringView, IConfigurable
  {
    private long minSize = 300 * 1024 * 1024;

    public override string Description => "Show only large files";

    public override string Name => "large";

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaInfo;
      if (i == null) {
        return false;
      }
      return i.InfoSize.HasValue && i.InfoSize.Value >= minSize;
    }

    public void SetParameters(ConfigParameters parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException(nameof(parameters));
      }

      long min;
      if (parameters.TryGet("size", out min)) {
        minSize = min * 1024 * 1024;
      }
    }
  }
}
