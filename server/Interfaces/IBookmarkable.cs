using System;

namespace NMaier.sdlna.Server
{
  public interface IBookmarkable
  {
    ulong? Bookmark { get; set; }
  }
}