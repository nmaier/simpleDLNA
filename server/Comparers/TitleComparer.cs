using System;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal class TitleComparer : BaseComparer
  {
    private static readonly StringComparer comparer =
      new NaturalStringComparer(false);

    public override string Description => "Sort alphabetically";

    public override string Name => "title";

    public override int Compare(IMediaItem x, IMediaItem y)
    {
      if (x == null && y == null) {
        return 0;
      }
      if (x == null) {
        return 1;
      }
      if (y == null) {
        return -1;
      }
      return comparer.Compare(x.ToComparableTitle(), y.ToComparableTitle());
    }
  }
}
