using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

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

    private static readonly ConcurrentQueue<Item> queue = CreateQueue();
    private static readonly AutoResetEvent signal = new AutoResetEvent(false);

    private static ConcurrentQueue<Item> CreateQueue()
    {
      new Thread(Run)
      {
        IsBackground = true,
        Priority = ThreadPriority.Lowest
      }.Start();

      return new ConcurrentQueue<Item>();
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
          try {
            if (store.HasCover(file)) {
              continue;
            }
            logger.DebugFormat("Trying {0}", file.Item.FullName);
            file.LoadCover();
            using (var k = file.Cover.Content) {
              k.ReadByte();
            }
          }
          catch (Exception) {
            // Already logged and don't care.
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
