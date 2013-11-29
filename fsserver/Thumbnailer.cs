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

    private static readonly BlockingCollection<Item> queue = CreateQueue();

    private static BlockingCollection<Item> CreateQueue()
    {
      new Thread(Run)
      {
        IsBackground = true,
        Priority = ThreadPriority.Lowest
      }.Start();

      return new BlockingCollection<Item>(new ConcurrentQueue<Item>());
    }

    private static void Run()
    {
      var logger = log4net.LogManager.GetLogger(typeof(Thumbnailer));
      logger.Debug("Thumber started");
      try {
        for (; ; ) {
          var item = queue.Take();
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
      finally {
        logger.Debug("Thumber stopped");
      }
    }

    public static void AddFiles(FileStore store, IEnumerable<WeakReference> items)
    {
      var storeRef = new WeakReference(store);
      foreach (var i in items) {
        queue.Add(new Item(storeRef, i));
      }
    }
  }
}
