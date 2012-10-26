using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace NMaier.sdlna.Util
{
  public class NaturalStringComparer : StringComparer
  {
    [DllImport("shlwapi.dll", CharSet = CharSet.Unicode)]
    private static extern int StrCmpLogicalW(string psz1, string psz2);
    private static readonly bool platformSupport;
    static NaturalStringComparer()
    {
      try {
        StrCmpLogicalW("a", "b");
        platformSupport = true;
      }
      catch (Exception) {
        // no op
      }
    }

    private static readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
    private class BasePart : IComparable<BasePart>
    {
      private readonly Type type;
      protected BasePart()
      {
        type = this.GetType();
      }
      public int CompareTo(BasePart other)
      {
        if (type == other.type) {
          if (other is StringPart) {
            return ((StringPart)this).CompareTo((StringPart)other);
          }
          return ((NumericPart)this).CompareTo((NumericPart)other);
        }
        if (type == typeof(StringPart)) {
          return 1;
        }
        return -1;
      }
    }
    private class StringPart : BasePart, IComparable<StringPart>
    {
      private readonly string str;
      public StringPart(string s)
      {
        str = s;
      }
      public int CompareTo(StringPart other)
      {
        return comparer.Compare(str, other.str);
      }
    }
    private class NumericPart : BasePart, IComparable<NumericPart>
    {
      private readonly int len;
      private readonly ulong val;
      public NumericPart(string s)
      {
        ulong.TryParse(s, out val);
        len = s.Length;
      }
      public int CompareTo(NumericPart other)
      {
        var rv = val.CompareTo(other.val);
        if (rv == 0) {
          return len.CompareTo(other.len);
        }
        return rv;
      }
    }

    private static Regex sanitizer = new Regex(@"\b(?:the|an?|ein(?:e[rs]?)?|der|die|das)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex whitespaces = new Regex(@"\s+", RegexOptions.Compiled);
    private static LRUCache<string, BasePart[]> cache = new LRUCache<string, BasePart[]>(5000);

    public NaturalStringComparer()
    { }

    private static int PartCompare(string left, string right)
    {
      int x, y;
      if (!int.TryParse(left, out x))
        return left.CompareTo(right);

      if (!int.TryParse(right, out y))
        return left.CompareTo(right);

      return x.CompareTo(y);
    }

    private BasePart[] Split(string str)
    {
      BasePart[] rv;
      if (cache.TryGetValue(str, out rv)) {
        return rv;
      }

      var s = Sanitize(str);
      var parts = new List<BasePart>();
      var num = false;
      int start = 0;
      for (int i = 0, end = s.Length; i < end; ++i) {
        var c = s[i];
        var cnum = c == ' ' ? num : c >= '0' && c <= '9';
        if (cnum == num) {
          continue;
        }
        if (i != 0) {
          var p = s.Substring(start, i - start).Trim();
          if (num) {
            parts.Add(new NumericPart(p));
          }
          else {
            parts.Add(new StringPart(p));
          }
        }
        num = cnum;
        start = i;
      }
      var pe = s.Substring(start).Trim();
      if (num && !string.IsNullOrEmpty(pe)) {
        parts.Add(new NumericPart(pe));
      }
      else {
        parts.Add(new StringPart(pe));
      }

      rv = parts.ToArray();
      lock (cache) {
        cache[str] = rv;
      }
      return rv;
    }

    private static string Sanitize(string str)
    {
      return whitespaces.Replace(sanitizer.Replace(str, ""), " ").Trim();
    }

    public override int Compare(string x, string y)
    {
      if (platformSupport) {
        return StrCmpLogicalW(Sanitize(x), Sanitize(y));
      }
      if (x == y || x.CompareTo(y) == 0) {
        return 0;
      }
      BasePart[] p1 = Split(x), p2 = Split(y);

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
