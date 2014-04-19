using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class BackgroundCacher
  {
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
      var logger = log4net.LogManager.GetLogger(typeof(BackgroundCacher));
      logger.Debug("started");
      var loadedSubTitles = 0ul;
      try {
        for (; ; ) {
          var item = queue.Take();
          var store = item.Store.Target as FileStore;
          var file = item.File.Target as BaseFile;
          if (store == null || file == null) {
            continue;
          }
          try {
            var mvi = file as IMetaVideoItem;
            if (mvi != null && mvi.SubTitle.HasSubtitle) {
              loadedSubTitles++;
            }
            if (store.HasCover(file)) {
              continue;
            }
            file.LoadCover();
            using (var k = file.Cover.Content) {
              k.ReadByte();
            }
          }
          catch (Exception) {
          }
        }
      }
      finally {
        logger.DebugFormat("stopped subtitles: {0}", loadedSubTitles);
      }
    }


    public static void AddFiles(FileStore store, IEnumerable<WeakReference> items)
    {
      var storeRef = new WeakReference(store);
      foreach (var i in items) {
        queue.Add(new Item(storeRef, i));
      }
    }


    private struct Item
    {
      public readonly WeakReference File;
      public readonly WeakReference Store;

      public Item(WeakReference store, WeakReference file)
      {
        Store = store;
        File = file;
      }
    }
  }
}
