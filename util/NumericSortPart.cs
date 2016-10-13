using System;

namespace NMaier.SimpleDlna.Utilities
{
  internal sealed class NumericSortPart : BaseSortPart, IComparable<NumericSortPart>
  {
    private readonly int len;

    private readonly ulong val;

    public NumericSortPart(string s)
    {
      val = ulong.Parse(s);
      len = s.Length;
    }

    public int CompareTo(NumericSortPart other)
    {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }
      var rv = val.CompareTo(other.val);
      if (rv == 0) {
        return len.CompareTo(other.len);
      }
      return rv;
    }
  }
}
