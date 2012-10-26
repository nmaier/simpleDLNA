using System;
using System.Collections;
using NMaier.sdlna.Server;
using NMaier.sdlna.Util;

namespace NMaier.sdlna.FileMediaServer.Comparers
{
  class TitleComparer : IItemComparer, IComparer
  {
    private static StringComparer comp = new NaturalStringComparer();
    public virtual string Description
    {
      get { return "Sort alphabetically"; }
    }

    public virtual string Name
    {
      get { return "title"; }
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