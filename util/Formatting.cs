using System.Text.RegularExpressions;
namespace NMaier.SimpleDlna.Utilities
{
  public static class Formatting
  {
    private readonly static Regex sanitizer = new Regex(@"\b(?:the|an?|ein(?:e[rs]?)?|der|die|das)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly static Regex whitespaces = new Regex(@"\s+|^[^\d\w]+|[^\d\w]+$|^[._+-]+|[._+-]+$", RegexOptions.Compiled);
    private readonly static Regex respace = new Regex(@"[._+-]+", RegexOptions.Compiled);

    public static string StemNameBase(this string str)
    {
      if (!str.Contains(" ")) {
        str = respace.Replace(str, " ").Trim();
      }
      var ws = whitespaces.Replace(str, " ").Trim();
      if (string.IsNullOrWhiteSpace(ws)) {
        return str;
      }
      return ws;
    }

    public static string StemCompareBase(this string str)
    {
      var san = sanitizer.Replace(str, string.Empty).Trim();
      if (string.IsNullOrWhiteSpace(san)) {
        return str;
      }
      return san.StemNameBase();
    }
    public static string FormatFileSize(this long size)
    {
      if (size < 900) {
        return string.Format("{0} B", size);
      }
      var ds = size / 1024.0;
      if (ds < 900) {
        return string.Format("{0:F2} KB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F2} MB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F3} GB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F3} TB", ds);
      }
      ds /= 1024.0;
      return string.Format("{0:F4} PB", ds);
    }
  }
}
