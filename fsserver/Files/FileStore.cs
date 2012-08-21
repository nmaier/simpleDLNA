using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Timers;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Files
{
  internal class DeserializeInfo
  {

    public FileInfo Info;
    public DlnaTypes Type;



    public DeserializeInfo(FileInfo aInfo, DlnaTypes aType)
    {
      Info = aInfo;
      Type = aType;
    }
  }

  internal sealed class FileStore : Logging, IDisposable
  {

    private readonly System.Data.IDbConnection connection;
    private readonly IDbCommand insert;
    private readonly IDbDataParameter insertData;
    private readonly IDbDataParameter insertKey;
    private readonly IDbDataParameter insertSize;
    private readonly IDbDataParameter insertTime;
    private readonly IDbCommand select;
    private readonly IDbDataParameter selectKey;
    private readonly IDbDataParameter selectSize;
    private readonly IDbDataParameter selectTime;
    private readonly Timer vacuumer = new Timer();

    internal FileStore(FileInfo aStore)
    {
      var cs = string.Format("Uri=file:{0}", aStore.FullName);
      if (aStore.Exists) {
        vacuumer.Interval = 120 * 1000;
      }
      else {
        vacuumer.Interval = 30 * 60 * 1000;
      }

      try {
        if (Type.GetType("Mono.Runtime") == null) {
          connection = new System.Data.SQLite.SQLiteConnection(cs);
        }
        else {
          Assembly monoSqlite;
          try {
            monoSqlite = Assembly.Load("Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
          }
          catch (Exception) {
            monoSqlite = Assembly.Load("Mono.Data.Sqlite, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
          }
          var dbconn = monoSqlite.GetType("Mono.Data.Sqlite.SqliteConnection");
          var ctor = dbconn.GetConstructor(new[] { typeof(string) });
          connection = ctor.Invoke(new[] { cs }) as IDbConnection;
        }
        if (connection == null) {
          throw new ArgumentNullException("no connection");
        }
        connection.Open();
      }
      catch (Exception ex) {
        Warn("FileStore is not availble; failed to load SQLite Adapter", ex);
        return;
      }

      using (var transaction = connection.BeginTransaction()) {
        using (var pragma = connection.CreateCommand()) {
          pragma.CommandText = "PRAGMA journal_mode = MEMORY";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA temp_store = MEMORY";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA locking_mode = EXCLUSIVE";
          pragma.ExecuteNonQuery();
          pragma.CommandText = "PRAGMA synchonous = NORMAL";
          pragma.ExecuteNonQuery();
        }
        using (var create = connection.CreateCommand()) {
          create.CommandText = "CREATE TABLE IF NOT EXISTS store (key TEXT PRIMARY KEY ON CONFLICT REPLACE, size INT, time INT, data BINARY)";
          create.ExecuteNonQuery();
        }
        transaction.Commit();
      }

      select = connection.CreateCommand();
      select.CommandText = "SELECT data FROM store WHERE key = ? AND size = ? AND time = ?";
      select.Parameters.Add(selectKey = select.CreateParameter());
      selectKey.DbType = DbType.String;
      select.Parameters.Add(selectSize = select.CreateParameter());
      selectSize.DbType = DbType.Int64;
      select.Parameters.Add(selectTime = select.CreateParameter());
      selectTime.DbType = DbType.Int64;

      insert = connection.CreateCommand();
      insert.CommandText = "INSERT INTO store VALUES(?,?,?,?)";
      insert.Parameters.Add(insertKey = select.CreateParameter());
      insertKey.DbType = DbType.String;
      insert.Parameters.Add(insertSize = select.CreateParameter());
      insertSize.DbType = DbType.Int64;
      insert.Parameters.Add(insertTime = select.CreateParameter());
      insertTime.DbType = DbType.Int64;
      insert.Parameters.Add(insertData = select.CreateParameter());
      insertData.DbType = DbType.Binary;

      InfoFormat("FileStore at {0} is ready", aStore.FullName);

      vacuumer.Elapsed += Vacuum;
      vacuumer.Enabled = true;
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
        connection.Dispose();
      }
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal BaseFile MaybeGetFile(BaseFolder aParent, FileInfo info, DlnaTypes type)
    {
      if (connection == null) {
        return null;
      }

      selectKey.Value = info.FullName;
      selectSize.Value = info.Length;
      selectTime.Value = info.LastWriteTimeUtc.Ticks;
      var data = select.ExecuteScalar() as byte[];
      if (data == null) {
        return null;
      }
      try {
        using (var s = new MemoryStream(data)) {
          var ctx = new StreamingContext(StreamingContextStates.Persistence, new DeserializeInfo(info, type));
          var formatter = new BinaryFormatter(null, ctx);
          formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
          formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
          var rv = formatter.Deserialize(s) as BaseFile;
          rv.Item = info;
          rv.Parent = aParent;
          return rv;
        }
      }
      catch (Exception ex) {
        Debug("Failed to deserialize an item", ex);
      }
      return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
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
          var ctx = new StreamingContext(StreamingContextStates.Persistence, null);
          var formatter = new BinaryFormatter(null, ctx);
          formatter.TypeFormat = FormatterTypeStyle.TypesWhenNeeded;
          formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
          formatter.Serialize(s, file);

          insertKey.Value = file.Item.FullName;
          insertSize.Value = file.Item.Length;
          insertTime.Value = file.Item.LastWriteTimeUtc.Ticks;
          insertData.Value = s.ToArray();
          insert.ExecuteNonQuery();
        }
      }
      catch (Exception ex) {
        Error("Failed to serialize an object of type " + file.GetType().ToString(), ex);
      }
    }

    public IDbTransaction BeginTransaction()
    {
      return connection.BeginTransaction();
    }

    private void Vacuum(object source, ElapsedEventArgs e)
    {
      Debug("Vacuuming");
      using (var q = connection.CreateCommand()) {
        q.CommandText = "VACUUM";
        try {
          q.ExecuteNonQuery();
        }
        catch (Exception ex) {
          Error("Failed to vacuum", ex);
        }
      }
      vacuumer.Interval = 30 * 60 * 1000;
    }

  }
}
