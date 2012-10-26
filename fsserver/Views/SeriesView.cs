using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NMaier.sdlna.FileMediaServer.Folders;
using System.Text.RegularExpressions;
using NMaier.sdlna.FileMediaServer.Files;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Views
{
  internal sealed class SeriesView : IView
  {
    private class SimpleKeyedVirtualFolder : KeyedVirtualFolder<VirtualFolder>
    {

      public SimpleKeyedVirtualFolder(FileServer server, BaseFolder aParent, string aName)
        : base(server, aParent, aName)
      {
      }

      public SimpleKeyedVirtualFolder() { }
    }

    public string Name
    {
      get { return  "series"; }
    }

    public string Description
    {
      get { return "Try to determine (TV) series from title and categorize accordingly"; }
    }
    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var root = Root as BaseFolder;
      var series = new SimpleKeyedVirtualFolder(Server, root, "Series");
      SortFolder(Server, root, series);
      foreach (var f in series.ChildFolders.ToList()) {
        if (f.ChildCount < 3) {
          continue;
        }
        var fsmi = f as VirtualFolder;
        fsmi.AdoptChildren();
        root.AdoptItem(fsmi);
        Server.RegisterPath(fsmi);
      }
    }

    static Regex re_series = new Regex(@"^(.+?)(?:s\d+[\s_-]*e\d+|\d+[\s_-]*x[\s_-]*\d+)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    static Regex re_sanitize = new Regex(@"^[^\w\d]+|[^\w\d]+$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private void SortFolder(FileServer server, BaseFolder folder, SimpleKeyedVirtualFolder series)
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
        series.GetFolder(ser).Link(vi);
      }
    }

    private string Sanitize(string s)
    {
      for (; ; ) {
        var i = s.Trim();
        s = re_sanitize.Replace(i, "").Trim();
        if (i == s) {
          return s;
        }
      }
    }
  }
}
