using System.Linq;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class SeriesView : CascadedView
  {
    private static readonly Regex regSeries = new Regex(
      @"^(.+?)(?:s\d+[\s_-]*e\d+|" + // S01E10
      @"\d+[\s_-]*x[\s_-]*\d+|" + // 1x01
      @"\b[\s-_]*(?:19|20|21)[0-9]{2}[\s._-](?:0[1-9]|1[012])[\s._-](?:0[1-9]|[12][0-9]|3[01])|" + // 2014.02.20
      @"\b[\s-_]*(?:0[1-9]|[12][0-9]|3[01])[\s._-](?:0[1-9]|1[012])[\s._-](?:19|20|21)[0-9]{2}|" + // 20.02.2014 (sane)
      @"\b[\s-_]*(?:0[1-9]|1[012])[\s._-](?:0[1-9]|[12][0-9]|3[01])[\s._-](?:19|20|21)[0-9]{2}|" + // 02.20.2014 (US)
      @"\b[1-9](?:0[1-9]|[1-3]\d)\b)", // 101
      RegexOptions.Compiled | RegexOptions.IgnoreCase
      );

    public override string Description => "Try to determine (TV) series from title and categorize accordingly";

    public override string Name => "series";

    protected override void SortFolder(IMediaFolder folder,
      SimpleKeyedVirtualFolder series)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(f, series);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        var title = i.Title;
        if (string.IsNullOrWhiteSpace(title)) {
          continue;
        }
        var m = regSeries.Match(title);
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
  }
}
