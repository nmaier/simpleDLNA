using System;
using System.Data;
using System.IO;
using System.Reflection;

namespace NMaier.SimpleDlna.Utilities
{
  public static class Sqlite
  {


    public static IDbConnection GetDatabaseConnection(FileInfo database)
    {
      var cs = string.Format("Uri=file:{0}", database.FullName);
      IDbConnection rv = null;
      if (Type.GetType("Mono.Runtime") == null) {
        rv = new System.Data.SQLite.SQLiteConnection(cs);
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
        rv = ctor.Invoke(new[] { cs }) as IDbConnection;
      }
      if (rv == null) {
        throw new ArgumentException("no connection");
      }
      rv.Open();
      return rv;
    }
  }
}
