using NMaier.SimpleDlna.Utilities;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class FilterView : FilteringView
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

    public override bool Allowed(IMediaResource res)
    {
      if (filter == null) {
        return true;
      }
      return filter.IsMatch(res.Title) || filter.IsMatch(res.Path);
    }

    private static string Escape(string str)
    {
      foreach (var c in "\\.+|[]{}()$#^".ToArray()) {
        var cs = new string(c, 1);
        str = str.Replace(cs, "\\" + cs);
      }
      if (str.Contains('*') || str.Contains("?")) {
        str = string.Format("^{0}$", str);
        str = str.Replace("*", ".*");
        str = str.Replace("?", ".");
      }
      return str;
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

      var filters = from f in parameters.Keys
                    let e = Escape(f)
                    select e;
      filter = new Regex(
        String.Join("|", filters),
        RegexOptions.Compiled | RegexOptions.IgnoreCase
      );
      NoticeFormat("Using filter {0}", filter.ToString());
    }

    public override IMediaFolder Transform(IMediaFolder root)
    {
      if (filter == null) {
        return root;
      }
      return base.Transform(root);
    }
  }
}
