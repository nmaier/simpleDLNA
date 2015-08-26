using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Linq;
using System.IO;
using RaptorDB;

namespace NMaier.SimpleDlna.FileStore.RaptorDB
{///Logging, 
  public sealed class FileStore : IFileStore, IDisposable
  {
    public string Description
    {
      get
      {
        return "RaptorDB file cache";
      }
    }

    public string Name
    {
      get
      {
        return "RaptorDB";
      }
    }

    FileInfo _storeFile;
    public string StoreFile { get { return _storeFile.FullName; } }

    private static readonly ILogging Logger = Logging.GetLogger<FileStore>();

    public void Dispose()
    {
      _db.Shutdown();
      _db.Dispose();
    }

    public override string ToString()
    {
      return string.Format("{0} - {1}", Name, Description);
    }

    public bool HasCover(IStoreItem file)
    {
      return MaybeGetCover(file) != null;
    }

    RaptorDB<string> _db;

    public const string DefaultFileName = "sdlna.cache.raptor.db";

    public void Init()
    {
      var storePath = _parameters.Keys.Contains("file") ? Convert.ToString(_parameters.GetValuesForKey("file").First()):DefaultFileName;
      if (!Path.IsPathRooted(storePath)) storePath = DataPath.Combine(storePath);
      _storeFile = new FileInfo(storePath);
      Logger.NoticeFormat("Opening [{0}] Store...", _storeFile);
      _db = RaptorDB<string>.Open(_storeFile.FullName, false);
    }

    public const string CoverPrefix = "Cover$$";

    public byte[] MaybeGetCover(IStoreItem file)
    {
      byte[] data;
      _db.Get(CoverPrefix + file.Item.FullName, out data);
      return data;
    }

    public byte[] MaybeGetFile(FileInfo info)
    {
      byte[] data;
      _db.Get(info.FullName, out data);
      return data;
    }

    public void MaybeStoreFile(IStoreItem file, byte[] data, byte[] coverData)
    {
     Logger.NoticeFormat("MaybeStoreFile [{0}][{1}][{2}]", file.Item.Name, (data == null)?0:data.Length, (coverData == null)?0:coverData.Length);
      _db.Set(file.Item.FullName, data);
      _db.Set(CoverPrefix + file.Item.FullName, coverData);
    }

    AttributeCollection _parameters = new AttributeCollection();

    public void SetParameters(AttributeCollection parameters)
    {
      _parameters = parameters;
    }
  }
}
