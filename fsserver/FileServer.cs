using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public sealed class FileServer
  : Logging, IMediaServer, IVolatileMediaServer, IDisposable
  {
    private readonly DirectoryInfo[] directories;

    private readonly ExtensionFilter filter;

    private readonly Identifiers ids;

    private bool isRescanning = false;

    private readonly static Random random = new Random();

    private readonly static StringComparer icomparer =
      StringComparer.CurrentCultureIgnoreCase;

    private readonly Timer changeTimer =
      new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);

    private readonly Guid uuid = Guid.NewGuid();

    private readonly Timer watchTimer =
      new Timer(TimeSpan.FromMinutes(random.Next(27, 33)).TotalMilliseconds);

    private readonly Regex re_sansitizeExt =
      new Regex(@"[^\w\d]+", RegexOptions.Compiled);

    private DateTime lastChanged = DateTime.Now;

    private static readonly double ChangeDefaultTime =
      TimeSpan.FromSeconds(30).TotalMilliseconds;

    private static readonly double ChangeRenamedTime =
      TimeSpan.FromSeconds(10).TotalMilliseconds;

    private static readonly double ChangeDeleteTime =
      TimeSpan.FromSeconds(2).TotalMilliseconds;

    private readonly List<WeakReference> pendingFiles =
      new List<WeakReference>();

    private bool rescanning = true;

    private FileStore store = null;

    private readonly DlnaMediaTypes types;

    private readonly FileSystemWatcher[] watchers;

    public FileServer(DlnaMediaTypes types, Identifiers ids,
                      params DirectoryInfo[] directories)
    {
      this.types = types;
      this.ids = ids;
      this.directories = directories.Distinct().ToArray();
      filter = new ExtensionFilter(this.types.GetExtensions());

      if (this.directories.Length == 0) {
        throw new ArgumentException(
          "Provide one or more directories",
          "directories"
          );
      }
      var parent = this.directories[0].Parent;
      if (parent == null) {
        parent = this.directories[0];
      }
      if (this.directories.Length == 1) {
        FriendlyName = string.Format(
          "{0} ({1})",
          this.directories[0].Name,
          parent.FullName
          );
      }
      else {
        FriendlyName = string.Format(
          "{0} ({1}) + {2}",
          this.directories[0].Name,
          parent.FullName,
          this.directories.Length - 1
          );
      }
      watchers = (from d in directories
                  select new FileSystemWatcher(d.FullName)).ToArray();
      uuid = DeriveUUID();
    }

    public event EventHandler Changed;

    public event EventHandler Changing;

    internal ExtensionFilter Filter
    {
      get
      {
        return filter;
      }
    }

    public IHttpAuthorizationMethod Authorizer
    {
      get;
      set;
    }

    public string FriendlyName { get; set; }

    public bool Rescanning
    {
      get
      {
        return rescanning;
      }
      set
      {
        if (rescanning == value) {
          return;
        }
        rescanning = value;
        if (rescanning) {
          Rescan();
        }
      }
    }

    public Guid Uuid
    {
      get
      {
        return uuid;
      }
    }

    private Guid DeriveUUID()
    {
      var bytes = Guid.NewGuid().ToByteArray();
      var i = 0;
      var copy = Encoding.ASCII.GetBytes("sdlnafs");
      for (; i < copy.Length; ++i) {
        bytes[i] = copy[i];
      }
      copy = Encoding.UTF8.GetBytes(FriendlyName);
      for (var j = 0; j < copy.Length && i < bytes.Length - 1; ++i, ++j) {
        bytes[i] = copy[j];
      }
      return new Guid(bytes);
    }

    private void DoRoot()
    {
      lock (this) {
        IMediaFolder newMaster;
        if (directories.Length == 1) {
          newMaster = new PlainRootFolder(this, directories[0]);
        }
        else {
          var virtualMaster = new VirtualFolder(
            null,
            FriendlyName,
            Identifiers.GeneralRoot
            );
          foreach (var d in directories) {
            virtualMaster.Merge(new PlainRootFolder(this, d));
          }
          newMaster = virtualMaster;
        }
        RegisterNewMaster(newMaster);
      }

      Thumbnail();
    }

    private bool HandleFileAdded(string fullPath)
    {
      lock (this) {
        var info = new FileInfo(fullPath);
        var item = ids.GetItemByPath(info.FullName) as IMediaResource;
        var folder = ids.GetItemByPath(info.Directory.FullName) as PlainFolder;
        if (item != null) {
          DebugFormat("Did find an existing {0}", info.FullName);
        }
        if (folder == null) {
          DebugFormat("Did not find folder for {0}", info.Directory.FullName);
          return false;
        }
        item = GetFile(folder, info);
        if (item == null) {
          DebugFormat("Failed to create new item for {0} - {1}", folder.Path, info.FullName);
          return false;
        }
        if (!Allowed(item)) {
          return true;
        }
        folder.AddResource(item);
        DebugFormat("Added {0} to corpus", item.Path);
        return true;
      }
    }

    private bool HandleFileDeleted(string fullPath)
    {
      lock (this) {
        var info = new FileInfo(fullPath);
        var item = ids.GetItemByPath(info.FullName) as IMediaResource;
        var folder = ids.GetItemByPath(info.Directory.FullName) as VirtualFolder;
        if (item == null || folder == null) {
          return false;
        }
        return folder.RemoveResource(item);
      }
    }

    private void OnChanged(Object source, FileSystemEventArgs e)
    {
      try {
        if (store != null &&
            icomparer.Equals(e.FullPath, store.StoreFile.FullName)) {
          return;
        }
        var ext = string.Empty;
        if (!string.IsNullOrEmpty(e.FullPath)) {
          ext = Path.GetExtension(e.FullPath);
          ext = string.IsNullOrEmpty(ext) ? string.Empty : ext.Substring(1);
        }
        if (!filter.Filtered(ext)) {
          DebugFormat(
            "Skipping name {0} {1}",
            e.Name, Path.GetExtension(e.FullPath));
          return;
        }
        DebugFormat(
          "File System changed ({1}): {0}", e.FullPath, e.ChangeType);
        var master = ids.GetItemById(Identifiers.GeneralRoot) as VirtualFolder;
        if (master != null) {
          switch (e.ChangeType) {
            case WatcherChangeTypes.Changed:
              if (HandleFileDeleted(e.FullPath) && HandleFileAdded(e.FullPath)) {
                ReaddRoot(master);
                return;
              }
              break;
            case WatcherChangeTypes.Created:
              if (HandleFileAdded(e.FullPath)) {
                ReaddRoot(master);
                return;
              }
              break;
            case WatcherChangeTypes.Deleted:
              if (HandleFileDeleted(e.FullPath)) {
                ReaddRoot(master);
                return;
              }
              break;
            default:
              break;
          }
        }
        DelayedRescan(e.ChangeType);
      }
      catch (Exception ex) {
        Error("OnChanged failed", ex);
      }
    }

    private void OnRenamed(Object source, RenamedEventArgs e)
    {
      try {
        var ext = string.Empty;
        if (!string.IsNullOrEmpty(e.FullPath)) {
          ext = Path.GetExtension(e.FullPath);
          ext = string.IsNullOrEmpty(ext) ? string.Empty : ext.Substring(1);
        }
        var ext2 = string.Empty;
        if (!string.IsNullOrEmpty(e.OldFullPath)) {
          ext2 = Path.GetExtension(e.OldFullPath);
          ext2 = string.IsNullOrEmpty(ext2) ? string.Empty : ext2.Substring(1);
        }
        if (!filter.Filtered(ext) && !filter.Filtered(ext2)) {
          DebugFormat(
            "Skipping name {0} {1} {2}",
            e.Name, ext, ext2);
          return;
        }
        DebugFormat(
          "File System changed (rename, {2}): {0} from {1}", e.FullPath, e.OldFullPath, e.ChangeType);
        var master = ids.GetItemById(Identifiers.GeneralRoot) as VirtualFolder;
        if (master != null) {
          var old = new FileInfo(e.OldFullPath);
          // XXX prefix
          if (directories.Contains(old.Directory)) {
            if (HandleFileDeleted(e.OldFullPath) && HandleFileAdded(e.FullPath)) {
              ReaddRoot(master);
              return;
            }
          }
          else {
            if (HandleFileAdded(e.FullPath)) {
              ReaddRoot(master);
              return;
            }
          }
        }
        DelayedRescan(e.ChangeType);
      }
      catch (Exception ex) {
        Error("OnRenamed failed", ex);
      }
    }

    private void ReaddRoot(VirtualFolder master)
    {
      RegisterNewMaster(master);
      if (Changed != null) {
        Changed.Invoke(this, EventArgs.Empty);
      }
    }

    private void RegisterNewMaster(IMediaFolder newMaster)
    {
      lock (ids) {
        ids.RegisterFolder(Identifiers.GeneralRoot, newMaster);
        ids.RegisterFolder(
          Identifiers.SamsungImages,
          new VirtualClonedFolder(
            newMaster,
            Identifiers.SamsungImages,
            types & DlnaMediaTypes.Image
            )
        );
        ids.RegisterFolder(
          Identifiers.SamsungAudio,
          new VirtualClonedFolder(
            newMaster,
            Identifiers.SamsungAudio,
            types & DlnaMediaTypes.Audio
            )
        );
        ids.RegisterFolder(
          Identifiers.SamsungVideo,
          new VirtualClonedFolder(
            newMaster,
            Identifiers.SamsungVideo,
            types & DlnaMediaTypes.Video
            )
        );
      }
    }

    private void RescanInternal()
    {
      lock (this) {
        if (!rescanning) {
          Debug("Rescanning disabled");
          return;
        }

        if (isRescanning) {
          Debug("Already rescanning");
        }
        isRescanning = true;
      }
      Task.Factory.StartNew(() =>
      {
        try {
          if (Changing != null) {
            Changing.Invoke(this, EventArgs.Empty);
          }

          try {
            NoticeFormat("Rescanning {0}...", FriendlyName);
            DoRoot();
            NoticeFormat("Done rescanning {0}...", FriendlyName);
          }
          catch (Exception ex) {
            Error(ex);
          }


          if (Changed != null) {
            Changed.Invoke(this, EventArgs.Empty);
          }
        }
        finally {
          lock (this) {
            isRescanning = false;
          }
        }
      },
      TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
    }

    private void RescanTimer(object sender, ElapsedEventArgs e)
    {
      RescanInternal();
    }

    private void Thumbnail()
    {
      if (store == null) {
        lock (this) {
          pendingFiles.Clear();
        }
        return;
      }
      lock (this) {
        DebugFormat(
          "Passing {0} files to background cacher", pendingFiles.Count);
        BackgroundCacher.AddFiles(store, pendingFiles);
        pendingFiles.Clear();
      }
    }

    internal bool Allowed(IMediaResource item)
    {
      return ids.Allowed(item);
    }

    internal void DelayedRescan(WatcherChangeTypes changeType)
    {
      if (changeTimer.Enabled) {
        return;
      }
      switch (changeType) {
        case WatcherChangeTypes.Deleted:
          changeTimer.Interval = ChangeDeleteTime;
          break;
        case WatcherChangeTypes.Renamed:
          changeTimer.Interval = ChangeRenamedTime;
          break;
        default:
          changeTimer.Interval = ChangeDefaultTime;
          break;
      }
      var diff = DateTime.Now - lastChanged;
      if (diff.TotalSeconds <= 30) {
        changeTimer.Interval = Math.Max(
          TimeSpan.FromSeconds(20).TotalMilliseconds,
          changeTimer.Interval
          );
        InfoFormat("Avoid thrashing {0}", changeTimer.Interval);
      }
      DebugFormat(
        "Change in {0} on {1}",
        changeTimer.Interval,
        FriendlyName
      );
      changeTimer.Enabled = true;
      lastChanged = DateTime.Now;
    }

    internal Cover GetCover(BaseFile file)
    {
      if (store != null) {
        return store.MaybeGetCover(file);
      }
      return null;
    }

    internal BaseFile GetFile(PlainFolder aParent, FileInfo info)
    {
      BaseFile item;
      lock (ids) {
        item = ids.GetItemByPath(info.FullName) as BaseFile;
      }
      if (item != null &&
          item.InfoDate == info.LastWriteTimeUtc &&
          item.InfoSize == info.Length) {
        return item;
      }

      var ext = re_sansitizeExt.Replace(
        info.Extension.ToUpperInvariant().Substring(1),
        string.Empty
        );
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];

      if (store != null) {
        item = store.MaybeGetFile(this, info, type);
        if (item != null) {
          lock (this) {
            pendingFiles.Add(new WeakReference(item));
          }
          return item;
        }
      }

      lock (this) {
        var rv = BaseFile.GetFile(aParent, info, type, mediaType);
        pendingFiles.Add(new WeakReference(rv));
        return rv;
      }
    }

    internal void UpdateFileCache(BaseFile aFile)
    {
      if (store != null) {
        store.MaybeStoreFile(aFile);
      }
    }

    public void Dispose()
    {
      foreach (var w in watchers) {
        w.Dispose();
      }
      if (changeTimer != null) {
        changeTimer.Dispose();
      }
      if (watchTimer != null) {
        watchTimer.Dispose();
      }
      if (store != null) {
        store.Dispose();
      }
      FileStreamCache.Clear();
    }

    public IMediaItem GetItem(string id)
    {
      lock (ids) {
        return ids.GetItemById(id);
      }
    }

    public void Load()
    {
      if (types == DlnaMediaTypes.Audio) {
        lock (ids) {
          if (!ids.HasViews) {
            ids.AddView("music");
          }
        }
      }
      DoRoot();

      changeTimer.AutoReset = false;
      changeTimer.Elapsed += RescanTimer;

      foreach (var watcher in watchers) {
        watcher.IncludeSubdirectories = true;
        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.EnableRaisingEvents = true;
      }

      watchTimer.Elapsed += RescanTimer;
      watchTimer.Enabled = true;
    }

    public void Rescan()
    {
      RescanInternal();
    }

    public void SetCacheFile(FileInfo info)
    {
      if (store != null) {
        store.Dispose();
        store = null;
      }
      try {
        store = new FileStore(info);
      }
      catch (Exception ex) {
        Warn("FileStore is not available; failed to load SQLite Adapter", ex);
        store = null;
      }
    }
  }
}
