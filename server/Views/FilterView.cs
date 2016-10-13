using System;
using System.Linq;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class FilterView : FilteringView, IConfigurable
  {
    private static readonly string[] escapes = "\\.+|[]{}()$#^".ToArray().Select(c => new string(c, 1)).ToArray();
    private Regex filter;

    public override string Description => "Show only files matching a specific filter";

    public override string Name => "filter";

    public override bool Allowed(IMediaResource res)
    {
      if (res == null) {
        throw new ArgumentNullException(nameof(res));
      }
      if (filter == null) {
        return true;
      }
      return filter.IsMatch(res.Title) || filter.IsMatch(res.Path);
    }

    private static string Escape(string str)
    {
      str = escapes.Aggregate(str, (current, cs) => current.Replace(cs, "\\" + cs));
      if (str.Contains('*') || str.Contains("?")) {
        str = $"^{str}$";
        str = str.Replace("*", ".*");
        str = str.Replace("?", ".");
      }
      return str;
    }

    public void SetParameters(ConfigParameters parameters)
    {
      if (parameters == null) {
        throw new ArgumentNullException(nameof(parameters));
      }

      var filters = from f in parameters.Keys
                    let e = Escape(f)
                    select e;
      filter = new Regex(
        string.Join("|", filters),
        RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
      NoticeFormat("Using filter {0}", filter.ToString());
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      if (filter == null) {
        return oldRoot;
      }
      return base.Transform(oldRoot);
    }
  }
}
