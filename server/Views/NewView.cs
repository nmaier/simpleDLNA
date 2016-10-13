using System;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class NewView : FilteringView, IConfigurable
  {
    private DateTime minDate = DateTime.Now.AddDays(-7.0);

    public override string Description => "Show only new files";

    public override string Name => "new";

    public override bool Allowed(IMediaResource res)
    {
      var i = res as IMetaInfo;
      if (i == null) {
        return false;
      }
      return i.InfoDate >= minDate;
    }

    public void SetParameters(ConfigParameters parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException(nameof(parameters));
      }

      foreach (var v in parameters.GetValuesForKey("date")) {
        DateTime min;
        if (DateTime.TryParse(v, out min)) {
          minDate = min;
        }
      }
    }
  }
}
