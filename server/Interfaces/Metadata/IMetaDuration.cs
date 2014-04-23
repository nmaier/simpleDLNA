using System;

namespace NMaier.SimpleDlna.Server.Metadata
{
  public interface IMetaDuration
  {
    TimeSpan? MetaDuration { get; }
  }
}
