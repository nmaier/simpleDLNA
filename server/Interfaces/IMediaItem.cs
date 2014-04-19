using System;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaItem : IComparable<IMediaItem>, IEquatable<IMediaItem>
  {
    string Id { get; set; }

    string Path { get; }

    IHeaders Properties { get; }

    string Title { get; }
  }
}
