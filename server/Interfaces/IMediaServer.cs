using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaServer
  {

    string FriendlyName { get; }

    IMediaFolder Root { get; }

    Guid Uuid { get; }



    IMediaItem GetItem(string id);
  }
}