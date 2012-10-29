using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server
{
  public interface IMediaFolder : IMediaItem
  {

    uint ChildCount { get; }

    IEnumerable<IMediaFolder> ChildFolders { get; }

    IEnumerable<IMediaResource> ChildItems { get; }

    IMediaFolder Parent { get; }
  }
}
