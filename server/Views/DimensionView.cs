using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class DimensionView : FilteringView
  {
    private uint? max;

    private uint? maxHeight;

    private uint? maxWidth;

    private uint? min;

    private uint? minHeight;

    private uint? minWidth;

    public override string Description
    {
      get
      {
        return "Show only items of a certain dimension";
      }
    }

    public override string Name
    {
      get
      {
        return "dimension";
      }
    }

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaResolution;
      if (i == null || !i.MetaWidth.HasValue || !i.MetaHeight.HasValue) {
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

    public override void SetParameters(AttributeCollection parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException("parameters");
      }
      base.SetParameters(parameters);

      min = SetParametersFor(parameters, "min");
      max = SetParametersFor(parameters, "max");
      minWidth = SetParametersFor(parameters, "minwidth");
      maxWidth = SetParametersFor(parameters, "maxwidth");
      minHeight = SetParametersFor(parameters, "minheight");
      maxHeight = SetParametersFor(parameters, "maxheight");
    }

    public static uint? SetParametersFor(AttributeCollection parameters,
                                         string key)
    {
      var val = (uint?)null;
      foreach (var v in parameters.GetValuesForKey(key)) {
        uint uv;
        if (uint.TryParse(v, out uv)) {
          val = uv;
        }
      }
      return val;
    }

    public override IMediaFolder Transform(IMediaFolder root)
    {
      if (!min.HasValue && !max.HasValue && !minWidth.HasValue &&
          !maxWidth.HasValue && !minHeight.HasValue && !maxHeight.HasValue) {
        return root;
      }
      return base.Transform(root);
    }
  }
}
