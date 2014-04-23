using NMaier.SimpleDlna.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class FilterView : BaseView
  {
    private Regex filter = null;

    public override string Description
    {
      get
      {
        return "Show only files matching a specific filter";
      }
    }

    public override string Name
    {
      get
      {
        return "filter";
      }
    }

    private void ProcessFolder(IMediaFolder root)
    {
      foreach (var f in root.ChildFolders) {
        ProcessFolder(f);
      }
      foreach (var f in root.ChildItems.ToList()) {
        if (filter.IsMatch(f.Title)) {
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
      if (parameters.Count == 0) {
        return;
      }

      var f = String.Join(",", parameters.Keys);
      f = f.Replace("*", ".*");
      f = f.Replace("?", ".?");
      f = f.Replace("\\", "\\\\");
      foreach (var c in "\\+|[]{}()$#^".ToArray()) {
        var cs = new string(c, 1);
        f = f.Replace(cs, "\\" + cs);
      }
      filter = new Regex(f, RegexOptions.Compiled | RegexOptions.IgnoreCase);
      NoticeFormat("Using filter {0}", f);
    }

    public override IMediaFolder Transform(IMediaFolder root)
    {
      if (filter == null) {
        return root;
      }
      root = new VirtualClonedFolder(root);
      ProcessFolder(root);
      return root;
    }
  }
}
