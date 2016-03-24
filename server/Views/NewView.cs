using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class NewView : FilteringView
  {
    private DateTime minDate = DateTime.Now.AddDays(-7.0);

    public override string Description
    {
      get
      {
        return "Show only new files";
      }
    }

    public override string Name
    {
      get
      {
        return "new";
      }
    }

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaInfo;
      if (i == null) {
        return false;
      }
      return i.InfoDate != null && i.InfoDate >= minDate;
    }

    public override void SetParameters(AttributeCollection parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException("parameters");
      }
      base.SetParameters(parameters);

      foreach (var v in parameters.GetValuesForKey("date")) {
        DateTime min;
        if (DateTime.TryParse(v, out min)) {
          minDate = min;
        }
      }
    }
  }
}
