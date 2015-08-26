﻿using NMaier.SimpleDlna.Server;
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

    private readonly static StringComparer icomparer =
      StringComparer.CurrentCultureIgnoreCase;

    private readonly Timer changeTimer =
      new Timer(TimeSpan.FromSeconds(20).TotalMilliseconds);

    private readonly Guid uuid = Guid.NewGuid();

    private readonly Timer watchTimer =
      new Timer(TimeSpan.FromMinutes(10).TotalMilliseconds);

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

    private readonly Identifiers ids;

    private IFileStore _store = null;
    private FileStoreReader _storeReader = null;
    private FileStoreWriter _storeWriter = null;

    private readonly DlnaMediaTypes types;

    private readonly FileSystemWatcher[] watchers;

    public FileServer(DlnaMediaTypes types, Identifiers ids,
                      params DirectoryInfo[] directories)
    {
      this.types = types;
      this.ids = ids;
      this.directories = directories.Distinct().ToArray();
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

    public IHttpAuthorizationMethod Authorizer
    {
      get;
      set;
    }

    public string FriendlyName { get; set; }

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
          newMaster = new PlainRootFolder(
            this,
            types,
            directories[0]
            );
        }
        else {
          var virtualMaster = new VirtualFolder(
            null,
            FriendlyName,
            Identifiers.GeneralRoot
            );
          foreach (var d in directories) {
            virtualMaster.Merge(
              new PlainRootFolder(this, types, d)
            );
          }
          newMaster = virtualMaster;
        }
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

      Thumbnail();
    }

    private void OnChanged(Object source, FileSystemEventArgs e)
    {
      try {
        //if (_store != null &&
        //    icomparer.Equals(e.FullPath, _store.StoreFile.FullName)) {
        //  return;
        //}
        var ext = string.IsNullOrEmpty(e.FullPath) ?
          Path.GetExtension(e.FullPath) :
          string.Empty;
        if (!string.IsNullOrEmpty(ext) &&
            !types.GetExtensions().Contains(
              ext.Substring(1), StringComparer.OrdinalIgnoreCase)) {
          DebugFormat(
            "Skipping name {0} {1} {2}",
            e.Name, Path.GetExtension(e.FullPath),
            string.Join(", ", types.GetExtensions()));
          return;
        }
        DebugFormat(
          "File System changed ({1}): {0}", e.FullPath, e.ChangeType);
        DelayedRescan(e.ChangeType);
      }
      catch (Exception ex) {
        Error("OnChanged failed", ex);
      }
    }

    private void OnRenamed(Object source, RenamedEventArgs e)
    {
      try {
        var exts = types.GetExtensions();
        var ext = string.IsNullOrEmpty(e.FullPath) ?
          Path.GetExtension(e.FullPath) :
          string.Empty;
        var c = StringComparer.OrdinalIgnoreCase;
        if (!string.IsNullOrEmpty(ext) &&
            !exts.Contains(ext.Substring(1), c) &&
            !exts.Contains(ext.Substring(1), c)) {
          DebugFormat(
            "Skipping name {0} {1} {2}",
            e.Name, Path.GetExtension(e.FullPath), string.Join(", ", exts));
          return;
        }
        DebugFormat(
          "File System changed ({1}): {0}", e.FullPath, e.ChangeType);
        DelayedRescan(e.ChangeType);
      }
      catch (Exception ex) {
        Error("OnRenamed failed", ex);
      }
    }

    private bool rescanning = true;
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

    private void RescanInternal()
    {
      if (!rescanning) {
        Debug("Rescanning disabled");
        return;
      }

      Task.Factory.StartNew(() =>
      {
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
      },
      TaskCreationOptions.AttachedToParent | TaskCreationOptions.LongRunning);
    }

    private void RescanTimer(object sender, ElapsedEventArgs e)
    {
      RescanInternal();
    }

    private void Thumbnail()
    {
      if (_store == null) {
        lock (this) {
          pendingFiles.Clear();
        }
        return;
      }
      lock (this) {
        DebugFormat(
          "Passing {0} files to background cacher", pendingFiles.Count);
        BackgroundCacher.AddFiles(_store, pendingFiles);
        pendingFiles.Clear();
      }
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
      if (_storeReader != null) {
        return _storeReader.GetCover(file);
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
        item.InfoDate == info.LastAccessTimeUtc &&
        item.InfoSize == info.Length) {
        return item;
      }

      var ext = re_sansitizeExt.Replace(
        info.Extension.ToUpperInvariant().Substring(1),
        string.Empty
        );
      var type = DlnaMaps.Ext2Dlna[ext];
      var mediaType = DlnaMaps.Ext2Media[ext];

      if (_store != null) {
        item = _storeReader.GetFile(info, this, type);
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
      if (_storeWriter != null) {
        _storeWriter.StoreFile(aFile);
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
      if (_store != null) {
        _store.Dispose();
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

    public void SetCacheFile(IFileStore store)
    {
      if (_store != null) {
        _store.Dispose();
        _store = null;
        _storeReader = null;
        _storeWriter = null;
      }
      try {
        _store = store;
        if (_store == null) return;
        _store.Init();
        _storeReader = new FileStoreReader(store);
        _storeWriter = new FileStoreWriter(store);
      }
      catch (Exception ex) {
        WarnFormat("FileStore is not available; failed to load [{0}]:{1}", store, ex);
        store = null;
      }
    }
  }
}
