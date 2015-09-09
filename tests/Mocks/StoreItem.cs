using NMaier.SimpleDlna.FileMediaServer;
using System;
using System.IO;

namespace SimpleDlna.Tests.Mocks
{
  [Serializable]
    public class StoreItemMock : IStoreItem
    {
      public StoreItemMock(FileInfo item) {
        Item = item;
      }

      public FileInfo Item { get; set; }

      public Cover MaybeGetCover() { return null; }
    }

}
