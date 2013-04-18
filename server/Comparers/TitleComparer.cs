using System;
using System.Collections;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal class TitleComparer : IItemComparer, IComparer
  {
    private readonly static StringComparer comp = new NaturalStringComparer();


    public virtual string Description
    {
      get
      {
        return "Sort alphabetically";
      }
    }
    public virtual string Name
    {
      get
      {
        return "title";
      }
    }


    public virtual int Compare(IMediaItem x, IMediaItem y)
    {
      if (x == null) {
        throw new ArgumentNullException("x");
      }
      if (y == null) {
        throw new ArgumentNullException("y");
      }
      return comp.Compare(x.Title, y.Title);
    }

    public int Compare(object x, object y)
    {
      if (x == null) {
        throw new ArgumentNullException("x");
      }
      if (y == null) {
        throw new ArgumentNullException("y");
      }
      return comp.Compare(x, y);
    }
  }
}
