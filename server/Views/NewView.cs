using System;
using System.Linq;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class NewView : BaseView
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
        if (i.InfoDate != null && i.InfoDate >= minDate) {
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

      foreach (var v in parameters.GetValuesForKey("date")) {
        DateTime min;
        if (DateTime.TryParse(v, out min)) {
          minDate = min;
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
