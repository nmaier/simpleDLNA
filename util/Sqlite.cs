using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace NMaier.SimpleDlna.Utilities
{
  [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Sqlite")]
  public static class Sqlite
  {
    private static Action<IDbConnection> clearPool = null;


    public static void ClearPool(IDbConnection conn)
    {
      if (clearPool != null) {
        clearPool(conn);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    public static IDbConnection GetDatabaseConnection(FileInfo database)
    {
      if (database == null) {
        throw new ArgumentNullException("database");
      }
      if (database.Exists && database.IsReadOnly) {
        throw new ArgumentException("Database file is read only", "database");
      }
      var cs = string.Format("Uri=file:{0}", database.FullName);
      if (Type.GetType("Mono.Runtime") == null) {
        var rv = new System.Data.SQLite.SQLiteConnection(cs);
        if (rv == null) {
          throw new ArgumentException("no connection");
        }
        rv.Open();
        if (clearPool == null) {
          clearPool = conn =>
          {
            System.Data.SQLite.SQLiteConnection.ClearPool(conn as System.Data.SQLite.SQLiteConnection);
          };
        }
        return rv;
      }

      Assembly monoSqlite;
      try {
        monoSqlite = Assembly.Load("Mono.Data.Sqlite, Version=4.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
      }
      catch (Exception) {
        monoSqlite = Assembly.Load("Mono.Data.Sqlite, Version=2.0.0.0, Culture=neutral, PublicKeyToken=0738eb9f132ed756");
      }
      var dbconn = monoSqlite.GetType("Mono.Data.Sqlite.SqliteConnection");
      var ctor = dbconn.GetConstructor(new[] { typeof(string) });
      var mrv = ctor.Invoke(new[] { cs }) as IDbConnection;
      if (mrv == null) {
        throw new ArgumentException("no connection");
      }
      mrv.Open();
      if (clearPool == null) {
        var cp = dbconn.GetMethod("ClearPool");
        clearPool = conn =>
        {
          if (cp != null) {
            cp.Invoke(null, new object[] { conn });
          }
        };
      }
      return mrv;
    }
  }
}
