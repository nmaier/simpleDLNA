using System;

namespace NMaier.SimpleDlna.Utilities
{
  internal sealed class StringSortPart
    : BaseSortPart, IComparable<StringSortPart>
  {
    private readonly StringComparer comparer;

    private readonly string str;

    internal StringSortPart(string str, StringComparer comparer)
    {
      this.str = str;
      this.comparer = comparer;
    }

    public int CompareTo(StringSortPart other)
    {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }
      return comparer.Compare(str, other.str);
    }
  }
}
