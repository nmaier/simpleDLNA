using System;

namespace NMaier.SimpleDlna.Server.Metadata
{
  public interface IMetaInfo
  {
    DateTime InfoDate { get; }

    long? InfoSize { get; }
  }
}
