using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public interface IFileStore : IDisposable, IRepositoryItem
  {
    void Init();
    string StoreFile { get; }
    bool HasCover(IStoreItem file);
    byte[] MaybeGetCover(IStoreItem file);
    byte[] MaybeGetFile(FileInfo info);
    void MaybeStoreFile(IStoreItem file, byte[] data, byte[] coverData);
  }
}
