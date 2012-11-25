using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Utilities
{
  internal abstract class BasePart : IComparable<BasePart>
  {

    private readonly Type type;



    protected BasePart()
    {
      type = this.GetType();
    }




    public int CompareTo(BasePart other)
    {
      if (type == other.type) {
        StringPart sp = other as StringPart;
        if (sp != null) {
          return ((StringPart)this).CompareTo(sp);
        }
        return ((NumericPart)this).CompareTo((NumericPart)other);
      }
      if (type == typeof(StringPart)) {
        return 1;
      }
      return -1;
    }
  }
  internal sealed class NumericPart : BasePart, IComparable<NumericPart>
  {

    private readonly int len;
    private readonly ulong val;



    public NumericPart(string s)
    {
      val = ulong.Parse(s);
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
  internal sealed class StringPart : BasePart, IComparable<StringPart>
  {

    private readonly StringComparer comparer;
    private readonly string str;



    public StringPart(string str, StringComparer comparer)
    {
      this.str = str;
      this.comparer = comparer;
    }




    public int CompareTo(StringPart other)
    {
      return comparer.Compare(str, other.str);
    }
  }

  public class NaturalStringComparer : StringComparer
  {

    private static LruDictionary<string, BasePart[]> cache = new LruDictionary<string, BasePart[]>(5000);
    private static readonly StringComparer comparer = StringComparer.CurrentCultureIgnoreCase;
    private static readonly bool platformSupport = HasPlatformSupport();
    private static Regex sanitizer = new Regex(@"\b(?:the|an?|ein(?:e[rs]?)?|der|die|das)\b", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    private static Regex whitespaces = new Regex(@"\s+", RegexOptions.Compiled);



    public NaturalStringComparer()
    { }




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

    static bool HasPlatformSupport()
    {
      try {
        return SafeNativeMethods.StrCmpLogicalW("a", "b") != 0;
      }
      catch (Exception) {
        // no op
      }
      return false;
    }

    private static string Sanitize(string str)
    {
      return whitespaces.Replace(sanitizer.Replace(str, ""), " ").Trim();
    }

    private static BasePart[] Split(string str)
    {
      BasePart[] rv;
      if (cache.TryGetValue(str, out rv)) {
        return rv;
      }

      var parts = new List<BasePart>();
      var num = false;
      int start = 0;
      for (int i = 0, end = str.Length; i < end; ++i) {
        var c = str[i];
        var cnum = c >= '0' && c <= '9';
        if (cnum == num) {
          continue;
        }
        if (i != 0) {
          var p = str.Substring(start, i - start).Trim();
          if (num) {
            parts.Add(new NumericPart(p));
          }
          else if (!string.IsNullOrWhiteSpace(p)) {
            parts.Add(new StringPart(p, comparer));
          }
        }
        num = cnum;
        start = i;
      }
      var pe = str.Substring(start).Trim();
      if (!string.IsNullOrWhiteSpace(pe)) {
        if (num) {
          parts.Add(new NumericPart(pe));
        }
        else {
          parts.Add(new StringPart(pe, comparer));
        }
      }

      rv = parts.ToArray();
      lock (cache) {
        cache[str] = rv;
      }
      return rv;
    }
  }
}
