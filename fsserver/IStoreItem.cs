using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public interface IStoreItem
  {
    FileInfo Item { get; set; }
    Cover MaybeGetCover();
  }
}
