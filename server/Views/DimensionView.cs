using System;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class DimensionView : FilteringView, IConfigurable
  {
    private uint? max;

    private uint? maxHeight;

    private uint? maxWidth;

    private uint? min;

    private uint? minHeight;

    private uint? minWidth;

    public override string Description => "Show only items of a certain dimension";

    public override string Name => "dimension";

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaResolution;
      if (i?.MetaWidth == null || !i.MetaHeight.HasValue) {
        return false;
      }
      var w = i.MetaWidth.Value;
      var h = i.MetaHeight.Value;
      if (min.HasValue && Math.Min(w, h) < min.Value) {
        return false;
      }
      if (max.HasValue && Math.Max(w, h) > max.Value) {
        return false;
      }
      if (minWidth.HasValue && w < minWidth.Value) {
        return false;
      }
      if (maxWidth.HasValue && w > maxWidth.Value) {
        return false;
      }
      if (minHeight.HasValue && h < minHeight.Value) {
        return false;
      }
      if (maxHeight.HasValue && h > maxHeight.Value) {
        return false;
      }
      return true;
    }

    public void SetParameters(ConfigParameters parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException(nameof(parameters));
      }
      min = parameters.MaybeGet<uint>("min");
      max = parameters.MaybeGet<uint>("max");
      minWidth = parameters.MaybeGet<uint>("minwidth");
      maxWidth = parameters.MaybeGet<uint>("maxwidth");
      minHeight = parameters.MaybeGet<uint>("minheight");
      maxHeight = parameters.MaybeGet<uint>("maxheight");
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      if (!min.HasValue && !max.HasValue && !minWidth.HasValue &&
          !maxWidth.HasValue && !minHeight.HasValue && !maxHeight.HasValue) {
        return oldRoot;
      }
      return base.Transform(oldRoot);
    }
  }
}
