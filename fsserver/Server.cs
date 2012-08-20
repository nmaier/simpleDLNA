using System;
using System.Collections.Generic;
using System.IO;
using System.Timers;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  public class FileServer : Logging, IMediaServer, IVolatileMediaServer
  {

    private readonly Timer changeTimer = new Timer(2000);
    private Comparers.IItemComparer comparer = null;
    private bool descending = false;
    private readonly DirectoryInfo directory;
    private readonly string friendlyName;
    private static readonly Random idGen = new Random();
    private readonly Dictionary<string, IMediaItem> ids = new Dictionary<string, IMediaItem>();
    private readonly Dictionary<string, string> paths = new Dictionary<string, string>();
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
      using (var trans = store.BeginTransaction()) {
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
        trans.Commit();
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

    internal Files.File GetFile(Folders.IFileServerFolder aParent, FileInfo aFile)
    {
      string key;
      if (paths.TryGetValue(aFile.FullName, out key)) {
        IMediaItem item;
        if (ids.TryGetValue(key, out item) && item is Files.File) {
          var ev = item as Files.File;
          if (ev.Parent is Folders.IFileServerFolder) {
            (ev.Parent as Folders.IFileServerFolder).ReleaseItem(ev);
          }
          ev.Parent = aParent;
          return ev;
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

      return Files.File.GetFile(aParent, aFile, type, mediaType);
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

    internal void UpdateFileCache(Files.File aFile)
    {
      if (store != null) {
        store.MaybeStoreFile(aFile);
      }
    }
  }
}
