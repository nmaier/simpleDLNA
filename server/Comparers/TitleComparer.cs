using System;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal class TitleComparer : BaseComparer
  {
    private readonly static StringComparer comp = new NaturalStringComparer(false);

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
      if (x == null) {
        throw new ArgumentNullException("x");
      }
      if (y == null) {
        throw new ArgumentNullException("y");
      }
      var tx = x as ITitleComparable;
      var ty = y as ITitleComparable;
      return comp.Compare(
        tx != null ? tx.ToComparableTitle() : x.Title.StemCompareBase(),
        ty != null ? ty.ToComparableTitle() : y.Title.StemCompareBase()
        );
    }
  }
}
