using System;
using System.Collections.Generic;
using System.Threading;
using NMaier.SimpleDlna.Utilities;
using System.Collections.Concurrent;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class Thumbnailer
  {
    private struct Item
    {
      public readonly WeakReference Store;
      public readonly WeakReference File;

      public Item(WeakReference store, WeakReference file)
      {
        Store = store;
        File = file;
      }
    }

    private static readonly Thread thread = new Thread(Run) { IsBackground = true, Priority = ThreadPriority.Lowest };
    private static readonly ConcurrentQueue<Item> queue = new ConcurrentQueue<Item>();
    private static readonly AutoResetEvent signal = new AutoResetEvent(false);

    static Thumbnailer()
    {
      thread.Start();
    }

    private static void Run()
    {
      var logger = log4net.LogManager.GetLogger(typeof(Thumbnailer));
      logger.Debug("thumber started");
      while (signal.WaitOne()) {
        Item item;
        while (queue.TryDequeue(out item)) {
          var store = item.Store.Target as FileStore;
          var file = item.File.Target as BaseFile;
          if (store == null || file == null) {
            continue;
          }
          logger.DebugFormat("Trying {0}", file.Item.FullName);
          try {
            if (store.HasCover(file)) {
              continue;
            }
            file.LoadCover();
            using (var k = file.Cover.Content) {
              k.ReadByte();
            }
          }
          catch (Exception ex) {
            logger.Warn(string.Format("Failed to cache thumb for {0}", file.Item.FullName), ex);
          }
        }
      }
    }

    public static void AddFiles(FileStore store, IEnumerable<WeakReference> items)
    {
      var storeRef = new WeakReference(store);
      foreach (var i in items) {
        queue.Enqueue(new Item(storeRef, i));
      }
      signal.Set();
    }
  }
}
