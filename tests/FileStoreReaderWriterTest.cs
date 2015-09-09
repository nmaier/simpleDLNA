using NUnit.Framework;
using NMaier.SimpleDlna.FileMediaServer;
using System.IO;
using System;
using NMaier.SimpleDlna.Utilities;
using NMaier.SimpleDlna.Server;

namespace SimpleDlna.Tests
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class FileStoreReaderTest
  {
    [Serializable]
    public class FileStoreMock : IFileStore
    {
      public string StoreFile
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string Description
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public string Name
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public void Dispose()
      {
      }

      public bool HasCover(IStoreItem file)
      {
        return _coverData != null;
      }

      public byte[] MaybeGetCover(IStoreItem file)
      {
        return _coverData;
      }

      byte[] _fileData = null;
      byte[] _coverData = null;

      public byte[] MaybeGetFile(FileInfo info)
      {
        return _fileData;
      }

      public void MaybeStoreFile(IStoreItem file, byte[] data, byte[] coverData)
      {
        _fileData = data;
        _coverData = coverData;
      }

      public void SetParameters(AttributeCollection parameters)
      {
        throw new NotImplementedException();
      }

      public void Init()
      {
        throw new NotImplementedException();
      }
    }

    [Serializable]
    public class BaseFileMock : BaseFile {
      public BaseFileMock(FileServer server, FileInfo file, DlnaMime type,
                       DlnaMediaTypes mediaType) : base(server,file,type,mediaType) {
      }
    }

    [Test]
    public void FileStoreReaderWriter_File_Roundtrip_Test()
    {
      var store = new FileStoreMock();
      var fi = new FileInfo(@"img\Patern_test.jpg");
      var reader = new FileStoreReader(store);
      var writer = new FileStoreWriter(store);
      var server = new FileServer(DlnaMediaTypes.All, null, new DirectoryInfo[] { new DirectoryInfo(".") });
      var item = new BaseFileMock(server,fi,DlnaMime.AudioAAC,DlnaMediaTypes.Image);
      writer.StoreFile(item);
      var result = reader.GetFile(fi, null, NMaier.SimpleDlna.Server.DlnaMime.ImageJPEG);
      Assert.IsNotNull(result);
      Assert.AreEqual(item.Path, result.Path);
    }
  }
}
