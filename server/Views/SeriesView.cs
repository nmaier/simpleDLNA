using NMaier.SimpleDlna.Utilities;
using System.Linq;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class SeriesView : BaseView
  {
    private readonly static Regex re_series = new Regex(
      @"^(.+?)(?:s\d+[\s_-]*e\d+|" + // S01E10
      @"\d+[\s_-]*x[\s_-]*\d+|" + // 1x01
      @"\b[\s-_]*(?:19|20|21)[0-9]{2}[\s._-](?:0[1-9]|1[012])[\s._-](?:0[1-9]|[12][0-9]|3[01])|" + // 2014.02.20
      @"\b[\s-_]*(?:0[1-9]|[12][0-9]|3[01])[\s._-](?:0[1-9]|1[012])[\s._-](?:19|20|21)[0-9]{2}|" + // 20.02.2014 (sane)
      @"\b[\s-_]*(?:0[1-9]|1[012])[\s._-](?:0[1-9]|[12][0-9]|3[01])[\s._-](?:19|20|21)[0-9]{2}|" + // 02.20.2014 (US)
      @"\b[1-9](?:0[1-9]|[1-3]\d)\b)", // 101
      RegexOptions.Compiled | RegexOptions.IgnoreCase
      );

    public override string Description
    {
      get
      {
        return "Try to determine (TV) series from title and categorize accordingly";
      }
    }

    public override string Name
    {
      get
      {
        return "series";
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
        var ser = m.Groups[1].Value;
        if (string.IsNullOrEmpty(ser)) {
          continue;
        }
        series.GetFolder(ser.StemNameBase()).AddResource(i);
        folder.RemoveResource(i);
      }
    }

    public override IMediaFolder Transform(IMediaFolder Root)
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
