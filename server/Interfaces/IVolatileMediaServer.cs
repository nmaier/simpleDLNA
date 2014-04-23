using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IVolatileMediaServer
  {
    void Rescan();

    event EventHandler Changed;
  }
}
