using log4net;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal class FileStreamCache
  {
    private readonly static ILog logger =
      LogManager.GetLogger(typeof(FileReadStream));

    private class CacheItem
    {
      public CacheItem(FileReadStream stream)
      {
        this.stream = stream;
        insertionPoint = DateTime.UtcNow;
      }
      public FileReadStream stream;
      public DateTime insertionPoint;
    }
    private static readonly LeastRecentlyUsedDictionary<string, CacheItem> streams = new LeastRecentlyUsedDictionary<string, CacheItem>(15);
    private readonly static Timer timer = new Timer(Cleanup, null, 20000, 5000);

    private static void Cleanup(object o)
    {
      lock (streams) {
        var keys = new List<string>(streams.Keys);
        foreach (var key in keys) {
          CacheItem item;
          if (streams.TryGetValue(key, out item)) {
            var diff = (DateTime.UtcNow - item.insertionPoint);
            if (diff.TotalMilliseconds > 2500) {
              logger.DebugFormat("Removed file stream {0} from cache", key);
              item.stream.Kill();
              streams.Remove(key);
            }
          }
        }
      }
    }

    internal static void Clear()
    {
      lock (streams) {
        foreach (var item in streams) {
          item.Value.stream.Kill();
        }
        streams.Clear();
      }
    }

    internal static FileReadStream Get(FileInfo info)
    {
      CacheItem rv;
      var key = info.FullName;
      lock (streams) {
        if (streams.TryGetValue(key, out rv)) {
          streams.Remove(key);
          logger.DebugFormat("Retrieved file stream {0} from cache", key);
          return rv.stream;
        }
      }
      logger.DebugFormat("Constructing file stream {0}", key);
      return new FileReadStream(info);
    }

    internal static void Recycle(FileReadStream stream)
    {
      try {
        var key = stream.Name;
        lock (streams) {
          CacheItem ignore;
          if (!streams.TryGetValue(key, out ignore) ||
            Object.Equals(ignore.stream, stream)) {
            logger.DebugFormat("Recycling {0}", key);
            stream.Seek(0, SeekOrigin.Begin);
            var removed = streams.AddAndPop(key, new CacheItem(stream));
            if (removed != null) {
              removed.stream.Kill();
            }
            return;
          }
        }
      }
      catch (Exception) {
        // no op
      }
      stream.Kill();
    }
  }

  internal sealed class FileReadStream : FileStream
  {
    private const int BUFFER_SIZE = 1 << 12;

    private readonly FileInfo info;

    private readonly static ILog logger =
      LogManager.GetLogger(typeof(FileReadStream));

    private bool killed = false;

    public FileReadStream(FileInfo info)
      : base(info.FullName, FileMode.Open,
             FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
             1,
             FileOptions.Asynchronous | FileOptions.SequentialScan)
    {
      this.info = info;
      logger.DebugFormat("Opened file {0}", this.info.FullName);
    }

    public void Kill()
    {
      logger.DebugFormat("Killed file {0}", info.FullName);
      killed = true;
      Close();
      Dispose();
    }

    public override void Close()
    {
      if (!killed) {
        FileStreamCache.Recycle(this);
        return;
      }
      base.Close();
      logger.DebugFormat("Closed file {0}", info.FullName);
    }

    protected override void Dispose(bool disposing)
    {
      if (!killed) {
        return;
      }
      base.Dispose(disposing);
    }
  }
}
