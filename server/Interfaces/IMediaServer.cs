using System;

namespace NMaier.sdlna.Server
{
  public interface IMediaServer
  {

    string FriendlyName { get; }

    IMediaFolder Root { get; }

    Guid UUID { get; }



    IMediaItem GetItem(string id);
  }
}