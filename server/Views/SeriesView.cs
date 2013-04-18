using System.Linq;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class SeriesView : IView
  {
    private readonly static Regex re_sanitize = new Regex(@"^[^\w\d]+|[^\w\d]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private readonly static Regex re_series = new Regex(@"^(.+?)(?:s\d+[\s_-]*e\d+|\d+[\s_-]*x[\s_-]*\d+|\b[1-9](?:0[1-9]|[1-3]\d)\b)", RegexOptions.Compiled | RegexOptions.IgnoreCase);


    public string Description
    {
      get
      {
        return "Try to determine (TV) series from title and categorize accordingly";
      }
    }
    public string Name
    {
      get
      {
        return "series";
      }
    }


    private static string Sanitize(string s)
    {
      for (; ; ) {
        var i = s.Trim();
        s = re_sanitize.Replace(i, string.Empty).Trim();
        if (i == s) {
          return s;
        }
      }
    }

    private static void SortFolder(IMediaFolder folder, SimpleKeyedVirtualFolder series)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(f, series);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        var title = i.Title;
        if (string.IsNullOrWhiteSpace(title)) {
          continue;
        }
        var m = re_series.Match(title);
        if (!m.Success) {
          continue;
        }
        var ser = Sanitize(m.Groups[1].Value);
        if (string.IsNullOrEmpty(ser)) {
          continue;
        }
        series.GetFolder(ser).AddResource(i);
        folder.RemoveResource(i);
      }
    }


    public IMediaFolder Transform(IMediaFolder Root)
    {
      var root = new VirtualClonedFolder(Root);
      var series = new SimpleKeyedVirtualFolder(root, "Series");
      SortFolder(root, series);
      foreach (var f in series.ChildFolders.ToList()) {
        if (f.ChildCount < 2) {
          foreach (var file in f.ChildItems) {
            root.AddResource(file);
          }
          continue;
        }
        var fsmi = f as VirtualFolder;
        root.AdoptFolder(fsmi);
      }
      return root;
    }


    private class SimpleKeyedVirtualFolder : KeyedVirtualFolder<VirtualFolder>
    {
      public SimpleKeyedVirtualFolder()
      {
      }
      public SimpleKeyedVirtualFolder(IMediaFolder aParent, string aName)
        : base(aParent, aName)
      {
      }
    }
  }
}
