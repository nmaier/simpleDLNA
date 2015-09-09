using NMaier.SimpleDlna.Utilities;
using NMaier.SimpleDlna.FileMediaServer;
using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Linq;

namespace NMaier.SimpleDlna.FileStore.SQLite
{//Logging, 
  public class FileStore : IFileStore
  {
    private const uint SCHEMA = 0x20140818;

    private IDbConnection connection;

    private IDbCommand insert;

    private IDbDataParameter insertCover;

    private IDbDataParameter insertData;

    private IDbDataParameter insertKey;

    private IDbDataParameter insertSize;

    private IDbDataParameter insertTime;

    private IDbCommand select;

    private IDbCommand selectCover;

    private IDbDataParameter selectCoverKey;

    private IDbDataParameter selectCoverSize;

    private IDbDataParameter selectCoverTime;

    private IDbDataParameter selectKey;

    private IDbDataParameter selectSize;

    private IDbDataParameter selectTime;

    private static readonly FileStoreVacuumer vacuumer =
      new FileStoreVacuumer();

    protected static readonly ILogging _logger = Logging.GetLogger<FileStore>();

    private readonly static object globalLock = new object();

    FileInfo _storeFile;
    public string StoreFile { get { return _storeFile.FullName; } }

    public string Description
    {
      get
      {
        return "SQLite database file cache";
      }
    }

    public string Name
    {
      get
      {
        return "SQLite";
      }
    }

    public const string DefaultFileName = "sdlna.cache.sqlite";

    public void Init() {
      _storeFile = new FileInfo(_parameters.Keys.Contains("file") ? Convert.ToString(_parameters.GetValuesForKey("file").First()):DefaultFileName);

      OpenConnection(_storeFile, out connection);
      SetupDatabase();

      select = connection.CreateCommand();
      select.CommandText =
        "SELECT data FROM store WHERE key = ? AND size = ? AND time = ?";
      select.Parameters.Add(selectKey = select.CreateParameter());
      selectKey.DbType = DbType.String;
      select.Parameters.Add(selectSize = select.CreateParameter());
      selectSize.DbType = DbType.Int64;
      select.Parameters.Add(selectTime = select.CreateParameter());
      selectTime.DbType = DbType.Int64;

      selectCover = connection.CreateCommand();
      selectCover.CommandText =
        "SELECT cover FROM store WHERE key = ? AND size = ? AND time = ?";
      selectCover.Parameters.Add(selectCoverKey = select.CreateParameter());
      selectCoverKey.DbType = DbType.String;
      selectCover.Parameters.Add(selectCoverSize = select.CreateParameter());
      selectCoverSize.DbType = DbType.Int64;
      selectCover.Parameters.Add(selectCoverTime = select.CreateParameter());
      selectCoverTime.DbType = DbType.Int64;

      insert = connection.CreateCommand();
      insert.CommandText =
        "INSERT OR REPLACE INTO store " +
        "VALUES(@key, @size, @time, @data, COALESCE(@cover, (SELECT cover FROM store WHERE key = @key)))";
      insert.Parameters.Add(insertKey = select.CreateParameter());
      insertKey.DbType = DbType.String;
      insertKey.ParameterName = "@key";
      insert.Parameters.Add(insertSize = select.CreateParameter());
      insertSize.DbType = DbType.Int64;
      insertSize.ParameterName = "@size";
      insert.Parameters.Add(insertTime = select.CreateParameter());
      insertTime.DbType = DbType.Int64;
      insertTime.ParameterName = "@time";
      insert.Parameters.Add(insertData = select.CreateParameter());
      insertData.DbType = DbType.Binary;
      insertData.ParameterName = "@data";
      insert.Parameters.Add(insertCover = select.CreateParameter());
      insertCover.DbType = DbType.Binary;
      insertCover.ParameterName = "@cover";

      _logger.InfoFormat("FileStore at {0} is ready", StoreFile);

      vacuumer.Add(connection);
    }

    public FileStore()
    {
    }

    private void OpenConnection(FileInfo storeFile,
                                out IDbConnection newConnection)
    {
      lock (globalLock) {
        newConnection = Sqlite.GetDatabaseConnection(storeFile);
        try {
          using (var ver = newConnection.CreateCommand()) {
            ver.CommandText = "PRAGMA user_version";
            var currentVersion = (uint)(long)ver.ExecuteScalar();
            if (!currentVersion.Equals(SCHEMA)) {
              throw new ArgumentOutOfRangeException("SCHEMA");
            }
          }
        }
        catch (Exception ex) {
          _logger.NoticeFormat(
            "Recreating database, schema update. ({0})",
            ex.Message
          );
          Sqlite.ClearPool(newConnection);
          newConnection.Close();
          newConnection.Dispose();
          newConnection = null;
          for (var i = 0; i < 10; ++i) {
            try {
              GC.Collect();
              storeFile.Delete();
              break;
            }
            catch (IOException) {
              Thread.Sleep(100);
            }
          }
          newConnection = Sqlite.GetDatabaseConnection(storeFile);
        }
        using (var pragma = connection.CreateCommand()) {
          pragma.CommandText = "PRAGMA journal_size_limt = 33554432";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA wal_autocheckpoint = 100";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA wal_checkpoint(TRUNCATE)";
          pragma.ExecuteNonQuery();
        }
      }
    }

    private void SetupDatabase()
    {
      using (var transaction = connection.BeginTransaction()) {
        using (var pragma = connection.CreateCommand()) {
          pragma.CommandText = string.Format(
            "PRAGMA user_version = {0}", SCHEMA);
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA page_size = 8192";
          pragma.ExecuteNonQuery();
        }
        using (var create = connection.CreateCommand()) {
          create.CommandText =
            "CREATE TABLE IF NOT EXISTS store (key TEXT PRIMARY KEY ON CONFLICT REPLACE, size INT, time INT, data BINARY, cover BINARY)";
          create.ExecuteNonQuery();
        }
        transaction.Commit();
      }
    }

    public bool HasCover(IStoreItem file)
    {
      if (connection == null) {
        return false;
      }

      var info = file.Item;
      lock (connection) {
        selectCoverKey.Value = info.FullName;
        selectCoverSize.Value = info.Length;
        selectCoverTime.Value = info.LastWriteTimeUtc.Ticks;
        try {
          var data = selectCover.ExecuteScalar();
          return (data as byte[]) != null;
        }
        catch (DbException ex) {
          _logger.Error("Failed to lookup file cover existence from store", ex);
          return false;
        }
      }
    }

    public byte[] MaybeGetCover(IStoreItem file)
    {
      if (connection == null) {
        return null;
      }

      var info = file.Item;
      //byte[] data;
      lock (connection) {
        selectCoverKey.Value = info.FullName;
        selectCoverSize.Value = info.Length;
        selectCoverTime.Value = info.LastWriteTimeUtc.Ticks;
        try {
          return selectCover.ExecuteScalar() as byte[];
        }
        catch (DbException ex) {
          _logger.Error("Failed to lookup file cover from store", ex);
          return null;
        }
      }
      //if (data == null) {
      //  return null;
      //}
      //try {
      //  using (var s = new MemoryStream(data)) {
      //    var ctx = new StreamingContext(
      //      StreamingContextStates.Persistence,
      //      new DeserializeInfo(null, info, DlnaMime.ImageJPEG)
      //      );
      //    var formatter = new BinaryFormatter(null, ctx) {
      //      TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
      //      AssemblyFormat = FormatterAssemblyStyle.Simple
      //    };
      //    var rv = formatter.Deserialize(s) as Cover;
      //    return rv;
      //  }
      //}
      //catch (SerializationException ex) {
      //  Debug("Failed to deserialize a cover", ex);
      //  return null;
      //}
      //catch (Exception ex) {
      //  Fatal("Failed to deserialize a cover", ex);
      //  throw;
      //}
    }

    public byte[] MaybeGetFile(
      //FileServer server, 
      FileInfo info//,
      //DlnaMime type
      )
    {
      if (connection == null) {
        return null;
      }
      //byte[] data;
      lock (connection) {
        selectKey.Value = info.FullName;
        selectSize.Value = info.Length;
        selectTime.Value = info.LastWriteTimeUtc.Ticks;
        try {
          return select.ExecuteScalar() as byte[];
        }
        catch (DbException ex) {
          _logger.Error("Failed to lookup file from store", ex);
          return null;
        }
      }
      //if (data == null) {
      //  return null;
      //}
      //try {
      //  using (var s = new MemoryStream(data)) {
      //    var ctx = new StreamingContext(
      //      StreamingContextStates.Persistence,
      //      new DeserializeInfo(server, info, type));
      //    var formatter = new BinaryFormatter(null, ctx) {
      //      TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
      //      AssemblyFormat = FormatterAssemblyStyle.Simple
      //    };
      //    var rv = formatter.Deserialize(s) as BaseFile;
      //    rv.Item = info;
      //    return rv;
      //  }
      //}
      //catch (Exception ex) {
      //  if (ex is TargetInvocationException || ex is SerializationException) {
      //    Debug("Failed to deserialize an item", ex);
      //    return null;
      //  }
      //  throw;
      //}
    }

    public void MaybeStoreFile(IStoreItem file, byte[] data, byte[] coverData)
    {
      if (connection == null) {
        return;
      }
      if (!file.GetType().Attributes.HasFlag(TypeAttributes.Serializable)) {
        return;
      }
      try {
        

          lock (connection) {
            insertKey.Value = file.Item.FullName;
            insertSize.Value = file.Item.Length;
            insertTime.Value = file.Item.LastWriteTimeUtc.Ticks;
            insertData.Value = data;

            insertCover.Value = coverData;
            try {
              insert.ExecuteNonQuery();
            }
            catch (DbException ex) {
              _logger.Error("Failed to put file cover into store", ex);
              return;
            }
          }
        
      }
      catch (Exception ex) {
        _logger.Error("Failed to serialize an object of type " + file.GetType(), ex);
        throw;
      }
    }

    public void Dispose()
    {
      if (insert != null) {
        insert.Dispose();
      }
      if (select != null) {
        select.Dispose();
      }
      if (connection != null) {
        vacuumer.Remove(connection);
        Sqlite.ClearPool(connection);
        connection.Dispose();
      }
    }

    AttributeCollection _parameters = new AttributeCollection();

    public void SetParameters(AttributeCollection parameters)
    {
      _parameters = parameters;
    }

    public override string ToString()
    {
      return string.Format("{0} - {1}", Name, Description);
    }

  }
}
