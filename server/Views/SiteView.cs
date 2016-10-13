using System;
using System.Linq;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class SiteView : CascadedView
  {
    private static readonly Regex regSites = new Regex(
      @"^[\[\(](?<site>.+?)[\]\)]|" +
      @"^(?<site>.+?)\s+-|" +
      @"^(?<site>.+?)[\[\]\(\)._-]|" +
      @"^(?<site>.+?)\s",
      RegexOptions.Compiled
      );
    private static readonly Regex regNumberStrip = new Regex(@"\d+$", RegexOptions.Compiled);

    private static readonly Regex regWord = new Regex(@"\w", RegexOptions.Compiled);

    public override string Description => "Try to determine websites from title and categorize accordingly";

    public override string Name => "sites";

    protected override void SortFolder(IMediaFolder folder,
      SimpleKeyedVirtualFolder series)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(f, series);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        try {
          var title = i.Title;
          if (string.IsNullOrWhiteSpace(title)) {
            throw new Exception("No title");
          }
          var m = regSites.Match(title);
          if (!m.Success) {
            throw new Exception("No match");
          }
          var site = m.Groups["site"].Value;
          if (string.IsNullOrEmpty(site)) {
            throw new Exception("No site");
          }
          site = site.Replace(" ", "").Replace("\t", "").Replace("-", "");
          site = regNumberStrip.Replace(site, string.Empty).TrimEnd();
          if (!regWord.IsMatch(site)) {
            throw new Exception("Not a site");
          }
          folder.RemoveResource(i);
          series.GetFolder(site.StemNameBase()).AddResource(i);
        }
        catch (Exception ex) {
          DebugFormat("{0} - {1}", ex.Message, i.Title);
          folder.RemoveResource(i);
          series.AddResource(i);
        }
      }
    }
  }
}
