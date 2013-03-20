using System;

namespace NMaier.SimpleDlna.Server
{
  [Flags]
  public enum DlnaMediaTypes : int
  {
    Audio = 1 << 2,
    Image = 1 << 1,
    Video = 1 << 0
  }
}
