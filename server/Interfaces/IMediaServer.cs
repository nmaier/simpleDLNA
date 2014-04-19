using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaServer
  {
    IHttpAuthorizationMethod Authorizer { get; }

    string FriendlyName { get; }

    Guid Uuid { get; }

    IMediaItem GetItem(string id);
  }
}
