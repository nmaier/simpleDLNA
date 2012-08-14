using System;

namespace NMaier.sdlna.Server
{
  public interface IMediaItemMetaData
  {

    DateTime ItemDate { get; }

    long ItemSize { get; }
  }
}
