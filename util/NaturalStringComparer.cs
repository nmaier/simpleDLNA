using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Utilities
{
  public class NaturalStringComparer : StringComparer
  {
    private readonly static LeastRecentlyUsedDictionary<string, BaseSortPart[]> cache = new LeastRecentlyUsedDictionary<string, BaseSortPart[]>(5000);

    private static readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;

    private static readonly bool platformSupport = HasPlatformSupport();

    private readonly static Regex sanitizer = new Regex(@"\b(?:the|an?|ein(?:e[rs]?)?|der|die|das)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);

    private readonly static Regex whitespaces = new Regex(@"\s+", RegexOptions.Compiled);


    private static bool HasPlatformSupport()
    {
      try {
        return SafeNativeMethods.StrCmpLogicalW("a", "b") != 0;
      }
      catch (Exception) {
        return false;
      }
    }

    private static string Sanitize(string str)
    {
      return whitespaces.Replace(sanitizer.Replace(str, string.Empty), " ").Trim();
    }

    private static BaseSortPart[] Split(string str)
    {
      BaseSortPart[] rv;
      if (cache.TryGetValue(str, out rv)) {
        return rv;
      }

      var parts = new List<BaseSortPart>();
      var num = false;
      var start = 0;
      for (int i = 0, end = str.Length; i < end; ++i) {
        var c = str[i];
        var cnum = c >= '0' && c <= '9';
        if (cnum == num) {
          continue;
        }
        if (i != 0) {
          var p = str.Substring(start, i - start).Trim();
          if (num) {
            parts.Add(new NumericSortPart(p));
          }
          else {
            if (!string.IsNullOrWhiteSpace(p)) {
              parts.Add(new StringSortPart(p, comparer));
            }
          }
        }
        num = cnum;
        start = i;
      }
      var pe = str.Substring(start).Trim();
      if (!string.IsNullOrWhiteSpace(pe)) {
        if (num) {
          parts.Add(new NumericSortPart(pe));
        }
        else {
          parts.Add(new StringSortPart(pe, comparer));
        }
      }

      rv = parts.ToArray();
      lock (cache) {
        cache[str] = rv;
      }
      return rv;
    }


    public override int Compare(string x, string y)
    {
      x = Sanitize(x);
      y = Sanitize(y);
      if (platformSupport) {
        return SafeNativeMethods.StrCmpLogicalW(x, y);
      }
      if (x == y || x.CompareTo(y) == 0) {
        return 0;
      }
      var p1 = Split(x);
      var p2 = Split(y);

      int rv;
      for (int i = 0, e = Math.Min(p1.Length, p2.Length); i < e; ++i) {
        rv = p1[i].CompareTo(p2[i]);
        if (rv != 0) {
          return rv;
        }
      }
      rv = p1.Length.CompareTo(p2.Length);
      if (rv == 0) {
        return comparer.Compare(x, y);
      }
      return rv;
    }

    public override bool Equals(string x, string y)
    {
      return Compare(x, y) == 0;
    }

    public override int GetHashCode(string obj)
    {
      return comparer.GetHashCode(obj);
    }
  }
}
