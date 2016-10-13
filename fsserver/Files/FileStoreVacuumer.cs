using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class FileStoreVacuumer : Logging, IDisposable
  {
    private const int MAX_TIME = 240 * 60 * 1000;

    private const int MIN_TIME = 30 * 60 * 1000;

    private readonly Dictionary<string, WeakReference> connections =
      new Dictionary<string, WeakReference>();

    private readonly Random rnd = new Random();

    private readonly Timer timer = new Timer();

    public FileStoreVacuumer()
    {
      timer.Elapsed += Run;
      Schedule();
    }

    public void Dispose()
    {
      timer?.Dispose();
    }

    private void Run(object sender, ElapsedEventArgs e)
    {
      IDbConnection[] conns;
      lock (connections) {
        conns = (from c in connections.Values
                 let conn = c.Target as IDbConnection
                 where conn != null
                 select conn).ToArray();
      }
      if (conns.Length == 0) {
        return;
      }

      Task.Factory.StartNew(() =>
      {
        foreach (var conn in conns) {
          try {
            Vacuum(conn);
          }
          catch (Exception ex) {
            Error("Failed to vacuum a store", ex);
          }
        }
      }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);

      Schedule();
    }

    private void Schedule()
    {
      timer.Interval = rnd.Next(MIN_TIME, MAX_TIME);
      timer.Enabled = true;
      DebugFormat("Scheduling next vaccuum in {0}", timer.Interval);
    }

    private void Vacuum(IDbConnection connection)
    {
      DebugFormat("VACUUM {0}", connection.Database);
      var files = new List<string>();

      lock (connection) {
        using (var q = connection.CreateCommand()) {
          q.CommandText = "SELECT key FROM store";
          using (var r = q.ExecuteReader()) {
            while (r.Read()) {
              files.Add(r.GetString(0));
            }
          }
        }
      }
      var gone = from f in files
                 let m = new FileInfo(f)
                 where !m.Exists
                 select f;
      lock (connection) {
        using (var trans = connection.BeginTransaction()) {
          using (var q = connection.CreateCommand()) {
            q.Transaction = trans;
            q.CommandText = "DELETE FROM store WHERE key = ?";
            var p = q.CreateParameter();
            p.DbType = DbType.String;
            q.Parameters.Add(p);
            foreach (var f in gone) {
              p.Value = f;
              lock (connection) {
                q.ExecuteNonQuery();
              }
              DebugFormat("Purging {0}", f);
            }
          }
        }
      }
      lock (connection) {
        using (var q = connection.CreateCommand()) {
          q.CommandText = "VACUUM";
          try {
            q.ExecuteNonQuery();
          }
          catch (Exception ex) {
            Error("Failed to vacuum", ex);
          }
        }
      }
      Debug("Vacuum done!");
    }

    public void Add(IDbConnection connection)
    {
      lock (connections) {
        connections[connection.ConnectionString] =
          new WeakReference(connection);
      }
    }

    public void Remove(IDbConnection connection)
    {
      lock (connections) {
        connections.Remove(connection.ConnectionString);
      }
    }
  }
}
