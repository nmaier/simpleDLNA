using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
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
    private readonly IDbCommand select;
    private readonly IDbDataParameter selectKey;



    internal FileStore(FileInfo aStore)
    {
      var cs = string.Format("Uri=file:{0}", aStore.FullName);

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
        using (var create = connection.CreateCommand()) {
          create.CommandText = "CREATE TABLE IF NOT EXISTS store (key TEXT PRIMARY KEY ON CONFLICT REPLACE, data BINARY)";
          create.ExecuteNonQuery();
        }
        transaction.Commit();
      }

      select = connection.CreateCommand();
      select.CommandText = "SELECT data FROM store WHERE key = ?";
      select.Parameters.Add(selectKey = select.CreateParameter());
      selectKey.DbType = DbType.String;

      insert = connection.CreateCommand();
      insert.CommandText = "INSERT INTO store VALUES(?,?)";
      insert.Parameters.Add(insertKey = select.CreateParameter());
      insertKey.DbType = DbType.String;
      insert.Parameters.Add(insertData = select.CreateParameter());
      insertData.DbType = DbType.Binary;

      InfoFormat("FileStore at {0} is ready", aStore.FullName);
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
    internal File MaybeGetFile(IFileServerFolder aParent, FileInfo info, DlnaTypes type)
    {
      if (connection == null) {
        return null;
      }

      selectKey.Value = info.FullName;
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
          var rv = formatter.Deserialize(s) as File;
          rv.Item = info;
          rv.Parent = aParent;
          return rv;
        }
      }
      catch (Exception ex) {
        Error("Failed to deserialize an item", ex);
      }
      return null;
    }

    [MethodImpl(MethodImplOptions.Synchronized)]
    internal void MaybeStoreFile(File file)
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
          using (var transaction = connection.BeginTransaction()) {
            insertKey.Value = file.Path;
            insertData.Value = s.ToArray();
            insert.ExecuteNonQuery();
            transaction.Commit();
          }
        }
      }
      catch (Exception ex) {
        Error("Failed to serialize an object of type " + file.GetType().ToString(), ex);
      }
    }
  }
}
