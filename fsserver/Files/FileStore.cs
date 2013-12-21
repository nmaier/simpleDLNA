using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class FileStore : Logging, IDisposable
  {
    private const uint SCHEMA = 0x20131101;

    private static readonly FileStoreVacuumer vacuumer =
      new FileStoreVacuumer();
    private readonly static object globalLock = new object();
    private readonly IDbConnection connection;
    private readonly IDbCommand insert;
    private readonly IDbDataParameter insertCover;
    private readonly IDbDataParameter insertData;
    private readonly IDbDataParameter insertKey;
    private readonly IDbDataParameter insertSize;
    private readonly IDbDataParameter insertTime;
    private readonly IDbCommand select;
    private readonly IDbDataParameter selectKey;
    private readonly IDbDataParameter selectSize;
    private readonly IDbDataParameter selectTime;
    private readonly IDbCommand selectCover;
    private readonly IDbDataParameter selectCoverKey;
    private readonly IDbDataParameter selectCoverSize;
    private readonly IDbDataParameter selectCoverTime;
    public readonly FileInfo StoreFile;

    internal FileStore(FileInfo storeFile)
    {
      StoreFile = storeFile;

      OpenConnection(storeFile, out connection);
      SetupDatabase();

      select = connection.CreateCommand();
      select.CommandText = "SELECT data FROM store WHERE key = ? AND size = ? AND time = ?";
      select.Parameters.Add(selectKey = select.CreateParameter());
      selectKey.DbType = DbType.String;
      select.Parameters.Add(selectSize = select.CreateParameter());
      selectSize.DbType = DbType.Int64;
      select.Parameters.Add(selectTime = select.CreateParameter());
      selectTime.DbType = DbType.Int64;

      selectCover = connection.CreateCommand();
      selectCover.CommandText = "SELECT cover FROM store WHERE key = ? AND size = ? AND time = ?";
      selectCover.Parameters.Add(selectCoverKey = select.CreateParameter());
      selectCoverKey.DbType = DbType.String;
      selectCover.Parameters.Add(selectCoverSize = select.CreateParameter());
      selectCoverSize.DbType = DbType.Int64;
      selectCover.Parameters.Add(selectCoverTime = select.CreateParameter());
      selectCoverTime.DbType = DbType.Int64;

      insert = connection.CreateCommand();
      insert.CommandText = "INSERT OR REPLACE INTO store VALUES(?,?,?,?,?)";
      insert.Parameters.Add(insertKey = select.CreateParameter());
      insertKey.DbType = DbType.String;
      insert.Parameters.Add(insertSize = select.CreateParameter());
      insertSize.DbType = DbType.Int64;
      insert.Parameters.Add(insertTime = select.CreateParameter());
      insertTime.DbType = DbType.Int64;
      insert.Parameters.Add(insertData = select.CreateParameter());
      insertData.DbType = DbType.Binary;
      insert.Parameters.Add(insertCover = select.CreateParameter());
      insertCover.DbType = DbType.Binary;

      InfoFormat("FileStore at {0} is ready", storeFile.FullName);

      vacuumer.Add(connection);
    }

    private void SetupDatabase()
    {
      using (var transaction = connection.BeginTransaction()) {
        using (var pragma = connection.CreateCommand()) {
          pragma.CommandText = string.Format("PRAGMA user_version = {0}", SCHEMA);
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA journal_mode = MEMORY";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA temp_store = MEMORY";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA synchonous = OFF";
          pragma.ExecuteNonQuery();
        }
        using (var create = connection.CreateCommand()) {
          create.CommandText = "CREATE TABLE IF NOT EXISTS store (key TEXT PRIMARY KEY ON CONFLICT REPLACE, size INT, time INT, data BINARY, cover BINARY)";
          create.ExecuteNonQuery();
        }
        transaction.Commit();
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2208:InstantiateArgumentExceptionsCorrectly")]
    private void OpenConnection(FileInfo storeFile, out IDbConnection newConnection)
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
          NoticeFormat(
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
        connection.Dispose();
      }
    }

    internal BaseFile MaybeGetFile(FileServer server, FileInfo info, DlnaMime type)
    {
      if (connection == null) {
        return null;
      }
      byte[] data;
      lock (connection) {
        selectKey.Value = info.FullName;
        selectSize.Value = info.Length;
        selectTime.Value = info.LastWriteTimeUtc.Ticks;
        try {
          data = select.ExecuteScalar() as byte[];
        }
        catch (DbException ex) {
          Error("Failed to lookup file from store", ex);
          return null;
        }
      }
      if (data == null) {
        return null;
      }
      try {
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            new DeserializeInfo(server, info, type));
          var formatter = new BinaryFormatter(null, ctx)
          {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          var rv = formatter.Deserialize(s) as BaseFile;
          rv.Item = info;
          return rv;
        }
      }
      catch (Exception ex) {
        if (ex is TargetInvocationException || ex is SerializationException) {
          Debug("Failed to deserialize an item", ex);
          return null;
        }
        throw;
      }
    }

    internal bool HasCover(BaseFile file)
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
          Error("Failed to lookup file cover existence from store", ex);
          return false;
        }
      }
    }

    internal Cover MaybeGetCover(BaseFile file)
    {
      if (connection == null) {
        return null;
      }

      var info = file.Item;
      byte[] data;
      lock (connection) {
        selectCoverKey.Value = info.FullName;
        selectCoverSize.Value = info.Length;
        selectCoverTime.Value = info.LastWriteTimeUtc.Ticks;
        try {
          data = selectCover.ExecuteScalar() as byte[];
        }
        catch (DbException ex) {
          Error("Failed to lookup file cover from store", ex);
          return null;
        }
      }
      if (data == null) {
        return null;
      }
      try {
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            new DeserializeInfo(null, info, DlnaMime.JPEG)
            );
          var formatter = new BinaryFormatter(null, ctx)
          {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          var rv = formatter.Deserialize(s) as Cover;
          return rv;
        }
      }
      catch (SerializationException ex) {
        Debug("Failed to deserialize a cover", ex);
        return null;
      }
      catch (Exception ex) {
        Fatal("Failed to deserialize a cover", ex);
        throw;
      }
    }

    internal void MaybeStoreFile(BaseFile file)
    {
      if (connection == null) {
        return;
      }
      if (!file.GetType().Attributes.HasFlag(TypeAttributes.Serializable)) {
        return;
      }
      try {
        using (var s = new MemoryStream()) {
          var ctx = new StreamingContext(
            StreamingContextStates.Persistence,
            null
            );
          var formatter = new BinaryFormatter(null, ctx)
          {
            TypeFormat = FormatterTypeStyle.TypesWhenNeeded,
            AssemblyFormat = FormatterAssemblyStyle.Simple
          };
          formatter.Serialize(s, file);

          lock (connection) {
            insertKey.Value = file.Item.FullName;
            insertSize.Value = file.Item.Length;
            insertTime.Value = file.Item.LastWriteTimeUtc.Ticks;
            insertData.Value = s.ToArray();

            var cover = file.MaybeGetCover();
            if (cover != null) {
              using (var c = new MemoryStream()) {
                formatter.Serialize(c, cover);
                insertCover.Value = c.ToArray();
              }
            }
            else {
              insertCover.Value = null;
            }

            try {
              insert.ExecuteNonQuery();
            }
            catch (DbException ex) {
              Error("Failed to put file cover into store", ex);
              return;
            }
          }
        }
      }
      catch (Exception ex) {
        Error("Failed to serialize an object of type " + file.GetType(), ex);
        throw;
      }
    }
  }
}
