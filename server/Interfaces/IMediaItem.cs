using System;

namespace NMaier.sdlna.Server
{
  public interface IMediaItem : IComparable<IMediaItem>
  {

    string ID { get; }

    IMediaFolder Parent { get; }

    string Title { get; }
  }
}