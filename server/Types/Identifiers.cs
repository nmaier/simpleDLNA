using System;
using System.Collections.Generic;
using System.Linq;
using NMaier.SimpleDlna.Server.Comparers;
using NMaier.SimpleDlna.Server.Views;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public sealed class Identifiers : Logging
  {
    public const string GENERAL_ROOT = "0";

    public const string SAMSUNG_AUDIO = "A";

    public const string SAMSUNG_IMAGES = "I";

    public const string SAMSUNG_VIDEO = "V";

    private static readonly Random idGen = new Random();

    private readonly IItemComparer comparer;
    private readonly List<IFilteredView> filters = new List<IFilteredView>();

    // ReSharper disable once CollectionNeverQueried.Local
    private readonly Dictionary<string, IMediaItem> hardRefs =
      new Dictionary<string, IMediaItem>();

    private readonly Dictionary<string, WeakReference> ids =
      new Dictionary<string, WeakReference>();

    private readonly bool order;

    private readonly List<IView> views = new List<IView>();

    private Dictionary<string, string> paths =
      new Dictionary<string, string>();

    public Identifiers(IItemComparer comparer, bool order)
    {
      this.comparer = comparer;
      this.order = order;
    }

    public bool HasViews => views.Count != 0;

    public IEnumerable<WeakReference> Resources => (from i in ids.Values
                                                    where i.Target is IMediaResource
                                                    select i).ToList();

    private void RegisterFolderTree(IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders) {
        RegisterFolderTree(f);
      }
      foreach (var i in folder.ChildItems) {
        RegisterPath(i);
      }
      RegisterPath(folder);
    }

    private void RegisterPath(IMediaItem item)
    {
      var path = item.Path;
      string id;
      if (!paths.ContainsKey(path)) {
        while (ids.ContainsKey(id = idGen.Next(1000, int.MaxValue).ToString("X8"))) {
        }
        paths[path] = id;
      }
      else {
        id = paths[path];
      }
      ids[id] = new WeakReference(item);

      item.Id = id;
    }

    public void AddView(string name)
    {
      try {
        var view = ViewRepository.Lookup(name);
        views.Add(view);
        var filter = view as IFilteredView;
        if (filter != null) {
          filters.Add(filter);
        }
      }
      catch (Exception ex) {
        Error("Failed to add view", ex);
        throw;
      }
    }

    public void Cleanup()
    {
      GC.Collect();
      var pc = paths.Count;
      var ic = ids.Count;
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
      DebugFormat("Cleanup complete: ids (evicted) {0} ({1}), paths {2} ({3})", ids.Count, ic - ids.Count, paths.Count,
                  pc - paths.Count);
    }

    public IMediaItem GetItemById(string id)
    {
      return ids[id].Target as IMediaItem;
    }

    public IMediaItem GetItemByPath(string path)
    {
      string id;
      if (!paths.TryGetValue(path, out id)) {
        return null;
      }
      return GetItemById(id);
    }

    public IMediaFolder RegisterFolder(string id, IMediaFolder item)
    {
      var rv = item;
      RegisterFolderTree(rv);
      foreach (var v in views) {
        rv = v.Transform(rv);
        RegisterFolderTree(rv);
      }
      rv.Cleanup();
      ids[id] = new WeakReference(rv);
      hardRefs[id] = rv;
      rv.Id = id;
      rv.Sort(comparer, order);
      return rv;
    }

    public bool Allowed(IMediaResource item)
    {
      return filters.All(f => f.Allowed(item));
    }
  }
}
