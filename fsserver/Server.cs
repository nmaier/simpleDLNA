using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Timers;
using NMaier.sdlna.Server;
using System.Threading.Tasks;

namespace NMaier.sdlna.FileMediaServer
{
  public class FileServer : Logging, IMediaServer, IVolatileMediaServer
  {

    private readonly Timer changeTimer = new Timer(2000);
    private Comparers.IItemComparer comparer = new Comparers.TitleComparer();
    private bool descending = false;
    private readonly DirectoryInfo directory;
    private readonly string friendlyName;
    private static readonly Random idGen = new Random();
    private Dictionary<string, IMediaItem> ids = new Dictionary<string, IMediaItem>();
    private Dictionary<string, string> paths = new Dictionary<string, string>();
    private IMediaFolder root;
    private Files.FileStore store = null;
    private readonly List<Views.IView> transformations = new List<Views.IView>();
    private MediaTypes types;
    private readonly Guid uuid = Guid.NewGuid();
    private readonly FileSystemWatcher watcher;



    public FileServer(MediaTypes aTypes, DirectoryInfo aDirectory)
    {
      types = aTypes;
      directory = aDirectory;
      friendlyName = directory.FullName;
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
      get { return root; }
    }

    public Guid UUID
    {
      get { return uuid; }
    }




    public event EventHandler Changed;




    public void AddView(string name)
    {
      transformations.Add(ViewRepository.Lookup(name));
    }

    public IMediaItem GetItem(string id)
    {
      return ids[id];
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
    }

    public void SetCacheFile(FileInfo info)
    {
      store = new Files.FileStore(info);
    }

    public void SetOrder(string order)
    {
      comparer = ComparerRepository.Lookup(order);
    }

    private void DoRoot()
    {
      // Collect some garbage
      var newPaths = new Dictionary<string, string>();
      var newIds = new Dictionary<string, IMediaItem>();
      foreach (var i in ids) {
        if (i.Value is Files.BaseFile && !(i.Value as Files.BaseFile).Item.Exists) {
          continue;
        }
        newIds.Add(i.Key, i.Value);
        newPaths.Add((i.Value as IFileServerMediaItem).Path, i.Key);
      }
      paths = newPaths;
      ids = newIds;

      var newRoot = new Folders.PlainRootFolder(this, types, directory);
      foreach (var t in transformations) {
        t.Transform(this, newRoot);
      }
      newRoot.Sort(comparer, descending);
      ids["0"] = root = newRoot;
#if DUMP_TREE
      using (var s = new FileStream("tree.dump", FileMode.Create, FileAccess.Write)) {
        using (var w = new StreamWriter(s)) {
          DumpTree(w, root);
        }
      }
#endif
      if (store != null) {
        var files = (from i in ids.Values
                     let f = (i as Files.BaseFile)
                     where f != null
                     select new WeakReference(f)).ToList();
        Task.Factory.StartNew(() =>
        {
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
          return;
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
      }
    }

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

    private void OnChanged(Object source, FileSystemEventArgs e)
    {
      if (store != null && e.FullPath.ToLower() == store.StoreFile.FullName.ToLower()) {
        return;
      }
      InfoFormat("File System changed: {0}", directory.FullName);
      changeTimer.Enabled = true;
    }

    private void OnRenamed(Object source, RenamedEventArgs e)
    {
      InfoFormat("File System changed (rename): {0}", directory.FullName);
      changeTimer.Enabled = true;
    }

    private void Rescan()
    {
      DoRoot();

      if (Changed != null) {
        Changed(this, null);
      }
    }

    private void RescanTimer(object sender, ElapsedEventArgs e)
    {
      Rescan();
    }

    internal Files.Cover GetCover(Files.BaseFile file)
    {
      if (store != null) {
        return store.MaybeGetCover(file);
      }
      return null;
    }

    internal Files.BaseFile GetFile(Folders.BaseFolder aParent, FileInfo aFile)
    {
      string key;
      if (paths.TryGetValue(aFile.FullName, out key)) {
        IMediaItem item;
        if (ids.TryGetValue(key, out item) && item is Files.BaseFile) {
          var ev = item as Files.BaseFile;
          if (ev.Parent is Folders.BaseFolder) {
            (ev.Parent as Folders.BaseFolder).ReleaseItem(ev);
          }
          if (ev.Date == aFile.LastWriteTimeUtc && ev.Size == aFile.Length) {
            ev.Parent = aParent;
            return ev;
          }
        }
      }

      var ext = aFile.Extension.ToLower().Substring(1);
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];

      if (store != null) {
        var sv = store.MaybeGetFile(aParent, aFile, type);
        if (sv != null) {
          return sv;
        }
      }

      return Files.BaseFile.GetFile(aParent, aFile, type, mediaType);
    }

    internal void RegisterPath(IFileServerMediaItem item)
    {
      var path = item.Path;
      string id;
      if (!paths.ContainsKey(path)) {
        while (ids.ContainsKey(id = idGen.Next(1000, int.MaxValue).ToString()))
          ;
        paths[path] = id;
      }
      else {
        id = paths[path];
      }
      ids[id] = item;
      item.ID = id;
    }

    internal void UpdateFileCache(Files.BaseFile aFile)
    {
      if (store != null) {
        store.MaybeStoreFile(aFile);
      }
    }
  }
}
