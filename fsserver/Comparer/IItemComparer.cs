using System.Collections.Generic;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  public interface IItemComparer : IComparer<IMediaItem>, IRepositoryItem { }
}