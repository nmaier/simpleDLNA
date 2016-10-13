using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Utilities
{
  using PartsCache = LeastRecentlyUsedDictionary<string, BaseSortPart[]>;

  public sealed class NaturalStringComparer : StringComparer
  {
    private static readonly StringComparer comparer =
      CurrentCultureIgnoreCase;

    private static readonly bool platformSupport = HasPlatformSupport();

    private readonly PartsCache partsCache = new PartsCache(5000);

    private readonly bool stemBase;

    public NaturalStringComparer()
      : this(false)
    {
    }

    public NaturalStringComparer(bool stemBase)
    {
      this.stemBase = stemBase;
    }

    public static IComparer<string> Comparer { get; } = new NaturalStringComparer();

    private static bool HasPlatformSupport()
    {
      try {
        return SafeNativeMethods.StrCmpLogicalW("a", "b") != 0;
      }
      catch (Exception) {
        return false;
      }
    }

    private BaseSortPart[] Split(string str)
    {
      BaseSortPart[] rv;
      lock (partsCache) {
        if (partsCache.TryGetValue(str, out rv)) {
          return rv;
        }
      }

      var parts = new List<BaseSortPart>();
      var num = false;
      var start = 0;
      for (var i = 0; i < str.Length; ++i) {
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
      lock (partsCache) {
        partsCache[str] = rv;
      }
      return rv;
    }

    public override int Compare(string x, string y)
    {
      if (stemBase) {
        x = x.StemCompareBase();
        y = y.StemCompareBase();
      }
      if (platformSupport) {
        return SafeNativeMethods.StrCmpLogicalW(x, y);
      }
      if (x == y || InvariantCulture.Compare(x, y) == 0) {
        return 0;
      }
      var p1 = Split(x);
      var p2 = Split(y);

      int rv;
      var e = Math.Min(p1.Length, p2.Length);
      for (var i = 0; i < e; ++i) {
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
