using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;

[assembly: CLSCompliant(true)]
namespace NMaier.SimpleDlna.FileMediaServer
{
  public sealed class FileServer : Logging, IMediaServer, IVolatileMediaServer, IDisposable
  {
    private readonly Timer changeTimer = new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);
    private readonly DirectoryInfo[] directories;
    private readonly Identifiers ids;
    private readonly string friendlyName;
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    private FileStore store = null;
    private Task thumberTask;
    private readonly DlnaMediaTypes types;
    private readonly Guid uuid = Guid.NewGuid();
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
    private readonly FileSystemWatcher[] watchers;
    private readonly Timer watchTimer = new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);
    private readonly Regex re_sansitizeExt = new Regex(@"[^\w\d]+", RegexOptions.Compiled);

    public FileServer(DlnaMediaTypes types, Identifiers ids, params DirectoryInfo[] directories)
    {
      this.types = types;
      this.ids = ids;
      this.directories = directories.Distinct().ToArray();
      if (this.directories.Length == 0) {
        throw new ArgumentException("Provide one or more directories", "directories");
      }
      if (this.directories.Length == 1) {
        friendlyName = string.Format("{0} ({1})", this.directories[0].Name, this.directories[0].Parent.FullName);
      }
      else {
        friendlyName = string.Format("{0} ({1}) + {2}", this.directories[0].Name, this.directories[0].Parent.FullName, this.directories.Length - 1);
      }
      watchers = (from d in directories
                  select new FileSystemWatcher(d.FullName)).ToArray();
      uuid = DeriveUUID();
    }

    public string FriendlyName
    {
      get
      {
        return friendlyName;
      }
    }

    public Guid Uuid
    {
      get
      {
        return uuid;
      }
    }

    public event EventHandler Changed;

    public void AddView(string name)
    {
      ids.AddView(name);
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
      if (thumberTask != null) {
        try {
          thumberTask.Dispose();
          thumberTask = null;
        }
        catch (ObjectDisposedException) {
          thumberTask = null; // Silence of the Warnings
        }
      }
    }

    public IMediaItem GetItem(string id)
    {
      return ids.GetItemById(id);
    }

    public void Load()
    {
      if (types == DlnaMediaTypes.Audio && !ids.HasViews) {
        ids.AddView("music");
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

    public void SetCacheFile(FileInfo info)
    {
      try {
        store = new FileStore(info);
      }
      catch (Exception ex) {
        Warn("FileStore is not availble; failed to load SQLite Adapter", ex);
        store = null;
      }
    }

    private Guid DeriveUUID()
    {
      byte[] bytes = Guid.NewGuid().ToByteArray();
      var i = 0;
      var copy = Encoding.ASCII.GetBytes("sdlnafs");
      for (; i < copy.Length; ++i) {
        bytes[i] = copy[i];
      }
      copy = Encoding.UTF8.GetBytes(friendlyName);
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
          newMaster = new PlainRootFolder(friendlyName, this, types, directories[0]);
        }
        else {
          var virtualMaster = new VirtualFolder(null, friendlyName, "0");
          foreach (var d in directories) {
            virtualMaster.Merge(new PlainRootFolder(friendlyName, this, types, d));
          }
          newMaster = virtualMaster;
        }
        ids.RegisterFolder("0", newMaster);
        ids.RegisterFolder("I", new VirtualClonedFolder(newMaster, "I", types & DlnaMediaTypes.Image));
        ids.RegisterFolder("A", new VirtualClonedFolder(newMaster, "A", types & DlnaMediaTypes.Audio));
        ids.RegisterFolder("V", new VirtualClonedFolder(newMaster, "V", types & DlnaMediaTypes.Video));
      }

      Thumbnail();
    }

    private void OnChanged(Object source, FileSystemEventArgs e)
    {
      if (changeTimer.Enabled) {
        return;
      }
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
      if (changeTimer.Enabled) {
        return;
      }

      DebugFormat("File System changed (rename): {0}", e.FullPath);
      changeTimer.Interval = TimeSpan.FromSeconds(10).TotalMilliseconds;
      changeTimer.Enabled = true;
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
      if (store == null) {
        return;
      }
      lock (ids) {
        if (thumberTask != null) {
          return;
        }
        var files = ids.Resources.ToList();
        thumberTask = new Task(() =>
        {
          using (thumberTask) {
            try {
              foreach (var i in files) {
                try {
                  var item = (i.Target as BaseFile);
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
          }
        }, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
        thumberTask.Start();
      }
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
      var item = ids.GetItemByPath(info.FullName) as BaseFile;
      if (item != null && item.InfoDate == info.LastAccessTimeUtc && item.InfoSize == info.Length) {
        return item;
      }

      var ext = re_sansitizeExt.Replace(info.Extension.ToLower().Substring(1), "");
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];

      if (store != null) {
        item = store.MaybeGetFile(this, info, type);
        if (item != null) {
          return item;
        }
      }

      return BaseFile.GetFile(aParent, info, type, mediaType);
    }

    internal void UpdateFileCache(BaseFile aFile)
    {
      if (store != null) {
        store.MaybeStoreFile(aFile);
      }
    }
  }
}
