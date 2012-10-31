using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public sealed class FileServer : Logging, IMediaServer, IVolatileMediaServer, IDisposable
  {

    private readonly Timer changeTimer = new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);
    private Comparers.IItemComparer comparer = new Comparers.TitleComparer();
    private bool descending = false;
    private readonly DirectoryInfo directory;
    private readonly string friendlyName;
    private static readonly Random idGen = new Random();
    private Dictionary<string, WeakReference> ids = new Dictionary<string, WeakReference>();
    private Folders.BaseFolder master;
    private Dictionary<string, string> paths = new Dictionary<string, string>();
    private Files.FileStore store = null;
    private Task thumberTask;
    private readonly List<Views.IView> transformations = new List<Views.IView>();
    private MediaTypes types;
    private readonly Guid uuid = Guid.NewGuid();
    private IMediaFolder root, images, audio, video;
    private readonly FileSystemWatcher watcher;
    private readonly Timer watchTimer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);



    public FileServer(MediaTypes types, DirectoryInfo directory)
    {
      this.types = types;
      this.directory = directory;
      friendlyName = string.Format("{0} ({1})", directory.Name, directory.Parent.FullName);
      watcher = new FileSystemWatcher(directory.FullName);
    }



    public bool DescendingOrder
    {
      get { return descending; }
      set { descending = value; }
    }

    public string FriendlyName
    {
      get { return friendlyName; }
    }

    public IMediaFolder Root
    {
      get { return master; }
    }

    public Guid Uuid
    {
      get { return uuid; }
    }




    public event EventHandler Changed;




    public void AddView(string name)
    {
      transformations.Add(ViewRepository.Lookup(name));
    }

    public void Dispose()
    {
      if (watcher != null) {
        watcher.Dispose();
      }
      if (changeTimer != null) {
        changeTimer.Dispose();
      }
      if (store != null) {
        store.Dispose();
      }
    }

    public IMediaItem GetItem(string id)
    {
      return ids[id].Target as IMediaItem;
    }

    public void Load()
    {
      if (types == MediaTypes.AUDIO && transformations.Count == 0) {
        AddView("music");
      }
      DoRoot();

      changeTimer.AutoReset = false;
      changeTimer.Elapsed += RescanTimer;

      watcher.IncludeSubdirectories = true;
      watcher.Created += new FileSystemEventHandler(OnChanged);
      watcher.Deleted += new FileSystemEventHandler(OnChanged);
      watcher.Renamed += new RenamedEventHandler(OnRenamed);
      watcher.EnableRaisingEvents = true;

      watchTimer.Elapsed += RescanTimer;
      watchTimer.Enabled = true;
    }

    public void SetCacheFile(FileInfo info)
    {
      store = new Files.FileStore(info);
    }

    public void SetOrder(string order)
    {
      comparer = ComparerRepository.Lookup(order);
    }

    private IMediaFolder BuildView(Folders.BaseFolder root)
    {
      var rv = root;
      foreach (var t in transformations) {
        rv = t.Transform(this, rv) as Folders.BaseFolder;
      }
      rv.Cleanup();
      rv.Sort(comparer, descending);
      return rv;
    }

    private void Cleanup()
    {
      GC.Collect();
      lock (ids) {
        int pc = paths.Count, ic = ids.Count;
        var npaths = new Dictionary<string, string>();
        foreach (var p in paths) {
          if (ids[p.Value].Target == null) {
            ids.Remove(p.Value);
          }
          else {
            npaths.Add(p.Key, p.Value);
          }
        }
        paths = npaths;
        DebugFormat("Cleanup complete: ids (evicted) {0} ({1}), paths {2} ({3})", ids.Count, ic - ids.Count, paths.Count, pc - paths.Count);
      }
    }

    private void DoRoot()
    {
      lock (ids) {
        master = new Folders.PlainRootFolder("0", this, types, directory);
        ids["0"] = new WeakReference(root = BuildView(master));
        RegisterFolderTree(master);

        images = BuildView(new Folders.VirtualClonedFolder(this, master, "I", types & MediaTypes.IMAGE));
        ids["I"] = new WeakReference(images);
        RegisterFolderTree(images);

        audio = BuildView(new Folders.VirtualClonedFolder(this, master, "A", types & MediaTypes.AUDIO));
        ids["A"] = new WeakReference(audio);
        RegisterFolderTree(audio);

        video = BuildView(new Folders.VirtualClonedFolder(this, master, "V", types & MediaTypes.VIDEO));
        ids["V"] = new WeakReference(video);
        RegisterFolderTree(video);
      }
      Cleanup();

#if DUMP_TREE
      using (var s = new FileStream("tree.dump", FileMode.Create, FileAccess.Write)) {
        using (var w = new StreamWriter(s)) {
          DumpTree(w, root);
        }
      }
#endif

      Thumbnail();
    }

    private void OnChanged(Object source, FileSystemEventArgs e)
    {
      if (store != null && e.FullPath.ToLower() == store.StoreFile.FullName.ToLower()) {
        return;
      }
      if (e.ChangeType == WatcherChangeTypes.Deleted) {
        changeTimer.Interval = TimeSpan.FromSeconds(2).TotalMilliseconds;
        changeTimer.Enabled = true;
        return;
      }
      DebugFormat("File System changed: {0}", e.FullPath);
      changeTimer.Interval = TimeSpan.FromSeconds(30).TotalMilliseconds;
      changeTimer.Enabled = true;
    }

    private void OnRenamed(Object source, RenamedEventArgs e)
    {
      DebugFormat("File System changed (rename): {0}", directory.FullName);
      changeTimer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
      changeTimer.Enabled = true;
    }

    private void RegisterFolderTree(IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders) {
        RegisterPath(f as IFileServerMediaItem);
        RegisterFolderTree(f);
      }
      foreach (var i in folder.ChildItems) {
        RegisterPath(i as IFileServerMediaItem);
      }
    }

    private void RegisterPath(IFileServerMediaItem item)
    {
      var path = item.Path;
      string id;
      if (!paths.ContainsKey(path)) {
        while (ids.ContainsKey(id = idGen.Next(1000, int.MaxValue).ToString("X8")))
          ;
        paths[path] = id;
      }
      else {
        id = paths[path];
      }
      ids[id] = new WeakReference(item);
      item.Id = id;
    }

    private void Rescan()
    {
      lock (this) {
        try {
          InfoFormat("Rescanning...");
          DoRoot();
          InfoFormat("Done rescanning...");

          if (Changed != null) {
            InfoFormat("Notifying...");
            Changed(this, null);
          }
        }
        catch (Exception ex) {
          Error(ex);
        }
      }
    }

    private void RescanTimer(object sender, ElapsedEventArgs e)
    {
      Rescan();
    }

    private void Thumbnail()
    {
      if (store != null && thumberTask == null) {
        var files = (from i in ids.Values
                     let f = (i.Target as Files.BaseFile)
                     where f != null
                     select new WeakReference(f)).ToList();
        thumberTask = Task.Factory.StartNew(() =>
        {
          try {
            foreach (var i in files) {
              try {
                var item = (i.Target as Files.BaseFile);
                if (item == null) {
                  continue;
                }
                if (store.HasCover(item)) {
                  continue;
                }
                item.LoadCover();
                using (var k = item.Cover.Content) {
                  k.ReadByte();
                }
              }
              catch (Exception ex) {
                Debug("Failed to thumb", ex);
              }
            }
          }
          catch (Exception ex) {
            Error(ex);
          }
          finally {
            thumberTask = null;
            GC.Collect();
          }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
      }
    }

    internal Files.Cover GetCover(Files.BaseFile file)
    {
      if (store != null) {
        return store.MaybeGetCover(file);
      }
      return null;
    }

    internal Files.BaseFile GetFile(Folders.BaseFolder aParent, FileInfo info)
    {
      string key;
      if (paths.TryGetValue(info.FullName, out key)) {
        WeakReference wr;
        Files.BaseFile file;
        if (ids.TryGetValue(key, out wr) && (file = wr.Target as Files.BaseFile) != null) {
          if (file.InfoDate == info.LastWriteTimeUtc && file.InfoSize == info.Length) {
            return file;
          }
        }
      }

      var ext = new Regex(@"[^\w\d]+", RegexOptions.Compiled).Replace(info.Extension.ToLower().Substring(1), "");
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];

      if (store != null) {
        var sv = store.MaybeGetFile(this, aParent, info, type);
        if (sv != null) {
          return sv;
        }
      }

      return Files.BaseFile.GetFile(aParent, info, type, mediaType);
    }

    internal void UpdateFileCache(Files.BaseFile aFile)
    {
      if (store != null) {
        store.MaybeStoreFile(aFile);
      }
    }


#if DUMP_TREE
    private void DumpTree(StreamWriter w, IMediaFolder folder, string prefix = "/")
    {
      foreach (IMediaFolder f in folder.ChildFolders) {
        w.WriteLine("{0} {1} - {2}", prefix, f.Title, f.GetType().ToString());
        DumpTree(w, f, prefix + f.Title + "/");
      }
      foreach (IMediaResource r in folder.ChildItems) {
        w.WriteLine("{0} {1} - {2}", prefix, r.Title, r.GetType().ToString());
      }
    }
#endif
  }
}
