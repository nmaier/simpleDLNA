using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IVolatileMediaServer
  {
    event EventHandler Changed;
  }
}
