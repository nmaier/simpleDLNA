using System;

namespace NMaier.SimpleDlna.Server
{
  [Flags]
  public enum DlnaMediaTypes
  {
    Audio = 1 << 2,
    Image = 1 << 1,
    Video = 1 << 0,
    All = ~(-1 << 3)
  }
}
