using NUnit.Framework;
using NMaier.SimpleDlna.FileMediaServer;
using System.IO;
using SimpleDlna.Tests.Mocks;

namespace SimpleDlna.Tests
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class FileStoreTest
  {
    [Test]
    public void FileStore_SQLite_File_Roundtrip_Test()
    {
      //var filename = "test.cache";
      IFileStore target = null;
      try {
        var fi = new FileInfo(@"img\Patern_test.jpg");
        target = new NMaier.SimpleDlna.FileStore.SQLite.FileStore();
        target.Init();
        var f1 = new StoreItemMock(fi);
        var data = File.ReadAllBytes(fi.FullName);
        var data2 = File.ReadAllBytes(fi.FullName);
        target.MaybeStoreFile(f1, data, data2);
        //var f1Cover = target.MaybeGetCover(f1);
        var f2 = target.MaybeGetFile(fi);
        Assert.IsNotNull(f2);
      }
      finally {
        if (target != null) {
          target.Dispose();
          if (File.Exists(target.StoreFile)) File.Delete(target.StoreFile);
        }
      }
    }

    [Test]
    public void FileStore_RaptorDB_File_Roundtrip_Test()
    {
      //var filename = "test.cache";
      IFileStore target = null;
      try {
        var fi = new FileInfo(@"img\Patern_test.jpg");
        target = new NMaier.SimpleDlna.FileStore.RaptorDB.FileStore();
        target.Init();
        var f1 = new StoreItemMock(fi);
        var data = File.ReadAllBytes(fi.FullName);
        var data2 = File.ReadAllBytes(fi.FullName);
        target.MaybeStoreFile(f1, data, data2);
        //var f1Cover = target.MaybeGetCover(f1);
        var f2 = target.MaybeGetFile(fi);
        Assert.IsNotNull(f2);
      }
      finally {
        if (target != null) {
          var filename = target.StoreFile;
          target.Dispose();
          if ((filename != null) && File.Exists(filename)) File.Delete(filename);
        }
      }
    }

  }
}
