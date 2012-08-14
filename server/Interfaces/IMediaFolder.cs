using System.Collections.Generic;

namespace NMaier.sdlna.Server
{
  public interface IMediaFolder : IMediaItem
  {

    uint ChildCount { get; }

    IEnumerable<IMediaFolder> ChildFolders { get; }
    IEnumerable<IMediaResource> ChildItems { get; }
  }
}
