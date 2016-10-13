using System;

namespace NMaier.SimpleDlna.Utilities
{
  internal abstract class BaseSortPart : IComparable<BaseSortPart>
  {
    private readonly Type type;

    protected BaseSortPart()
    {
      type = GetType();
    }

    public int CompareTo(BaseSortPart other)
    {
      if (other == null) {
        return 1;
      }
      if (type != other.type) {
        if (type == typeof (StringSortPart)) {
          return 1;
        }
        return -1;
      }
      var sp = other as StringSortPart;
      if (sp != null) {
        return ((StringSortPart)this).CompareTo(sp);
      }
      return ((NumericSortPart)this).CompareTo(
        (NumericSortPart)other);
    }
  }
}
