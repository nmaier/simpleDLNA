using NMaier.SimpleDlna.Utilities;
using System;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal class TitleComparer : BaseComparer
  {
    private readonly static StringComparer comparer =
      new NaturalStringComparer(false);

    public override string Description
    {
      get
      {
        return "Sort alphabetically";
      }
    }

    public override string Name
    {
      get
      {
        return "title";
      }
    }

    public override int Compare(IMediaItem x, IMediaItem y)
    {
      if (x == null && y == null) {
        return 0;
        throw new ArgumentNullException("x");
      }
      if (x == null) {
        return 1;
      }
      if (y == null) {
        return -1;
      }
      var tx = x as ITitleComparable;
      var ty = y as ITitleComparable;
      return comparer.Compare(
        tx != null ? tx.ToComparableTitle() : x.Title.StemCompareBase(),
        ty != null ? ty.ToComparableTitle() : y.Title.StemCompareBase()
        );
    }
  }
}
