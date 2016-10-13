using System;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using log4net;

namespace NMaier.SimpleDlna.Utilities
{
  public static class Sqlite
  {
    private const int GROW_SIZE = 1 << 24;

    private static Action<IDbConnection> clearPool;

    private static IDbConnection GetDatabaseConnectionMono(string cs)
    {
      Assembly monoSqlite;
      try {
        monoSqlite = Assembly.Load(
          "Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
      }
      catch (Exception) {
        monoSqlite = Assembly.Load(
          "Mono.Data.Sqlite, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
      }
      var dbconn = monoSqlite.GetType(
        "Mono.Data.Sqlite.SqliteConnection");
      var ctor = dbconn.GetConstructor(new[] {typeof (string)});
      if (ctor == null) {
        throw new ArgumentException("No mono SQLite found");
      }
      var rv = ctor.Invoke(new object[] {cs}) as IDbConnection;
      if (rv == null) {
        throw new ArgumentException("no connection");
      }
      rv.Open();
      if (clearPool == null) {
        var cp = dbconn.GetMethod("ClearPool");
        clearPool = conn =>
        {
          cp?.Invoke(null, new object[] {conn});
        };
      }
      return rv;
    }

    private static IDbConnection GetDatabaseConnectionSDS(string cs)
    {
      var rv = new SQLiteConnection(cs);
      if (rv == null) {
        throw new ArgumentException("no connection");
      }
      rv.Open();

      try {
        rv.SetChunkSize(GROW_SIZE);
      }
      catch (Exception ex) {
        LogManager.GetLogger(typeof (Sqlite)).Error(
          "Failed to sqlite control", ex);
      }

      if (clearPool == null) {
        clearPool = conn =>
        {
          SQLiteConnection.ClearPool(
            conn as SQLiteConnection);
        };
      }
      return rv;
    }

    public static void ClearPool(IDbConnection conn)
    {
      clearPool?.Invoke(conn);
    }

    public static IDbConnection GetDatabaseConnection(FileInfo database)
    {
      if (database == null) {
        throw new ArgumentNullException(nameof(database));
      }
      if (database.Exists && database.IsReadOnly) {
        throw new ArgumentException(
          "Database file is read only",
          nameof(database)
          );
      }
      var cs = $"Uri=file:{database.FullName};Pooling=true;Synchronous=Off;journal mode=TRUNCATE;DefaultTimeout=5";

      if (SystemInformation.IsRunningOnMono()) {
        return GetDatabaseConnectionMono(cs);
      }
      return GetDatabaseConnectionSDS(cs);
    }
  }
}
