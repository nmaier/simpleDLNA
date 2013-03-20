using System.Linq;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.FileMediaServer.Files;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Views
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

    private static void SortFolder(FileServer server, BaseFolder folder, SimpleKeyedVirtualFolder series)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(server, f as BaseFolder, series);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        var vi = i as VideoFile;
        if (vi == null) {
          continue;
        }
        var title = vi.Title;
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
        series.GetFolder(ser).AddFile(vi);
        folder.RemoveFile(vi);
      }
    }


    public IMediaFolder Transform(FileServer Server, IMediaFolder Root)
    {
      var root = new VirtualClonedFolder(Root as BaseFolder);
      var series = new SimpleKeyedVirtualFolder(Server, root, "Series");
      SortFolder(Server, root, series);
      foreach (var f in series.ChildFolders.ToList()) {
        if (f.ChildCount < 2) {
          foreach (var file in f.ChildItems) {
            root.AddFile(file as BaseFile);
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
      public SimpleKeyedVirtualFolder(FileServer server, BaseFolder aParent, string aName)
        : base(server, aParent, aName)
      {
      }
    }
  }
}
