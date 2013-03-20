using System;
using System.Collections;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer.Comparers
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
      return comp.Compare(x.Title, y.Title);
    }

    public int Compare(object x, object y)
    {
      return comp.Compare(x, y);
    }
  }
}
