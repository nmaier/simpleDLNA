using System;

namespace NMaier.sdlna.Server.Metadata
{
  public interface IMetaInfo
  {

    DateTime Date { get; }

    long? Size { get; }
  }
}
