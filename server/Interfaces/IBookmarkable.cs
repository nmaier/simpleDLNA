using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IBookmarkable
  {

    ulong? Bookmark { get; set; }
  }
}