using System;
using System.Collections.Generic;
using System.Linq;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  public class VirtualFolder : IMediaFolder
  {
    private static readonly StringComparer comparer =
      new NaturalStringComparer(true);

    private readonly List<IMediaFolder> merged = new List<IMediaFolder>();
    private string comparableTitle;

    protected List<IMediaFolder> Folders = new List<IMediaFolder>();

    private string path;

    protected List<IMediaResource> Resources = new List<IMediaResource>();

    public VirtualFolder()
    {
    }

    public VirtualFolder(IMediaFolder parent, string name)
      : this(parent, name, name)
    {
    }

    public VirtualFolder(IMediaFolder parent, string name, string id)
    {
      Parent = parent;
      Id = id;
      Name = name;
    }

    public IEnumerable<IMediaResource> AllItems
    {
      get {
        return Folders.SelectMany(f => ((VirtualFolder)f).AllItems).
          Concat(Resources);
      }
    }

    public string Name { get; set; }

    public int ChildCount => Folders.Count + Resources.Count;

    public int FullChildCount => Resources.Count + (from f in Folders select f.FullChildCount).Sum();

    public IEnumerable<IMediaFolder> ChildFolders => Folders;

    public IEnumerable<IMediaResource> ChildItems => Resources;

    public string Id { get; set; }

    public IMediaFolder Parent { get; set; }

    public virtual string Path
    {
      get {
        if (!string.IsNullOrEmpty(path)) {
          return path;
        }
        var p = string.IsNullOrEmpty(Id) ? Name : Id;
        if (Parent != null) {
          var vp = Parent as VirtualFolder;
          path = $"{(vp != null ? vp.Path : Parent.Id)}/v:{p}";
        }
        else {
          path = p;
        }
        return path;
      }
    }

    public IHeaders Properties
    {
      get {
        var rv = new RawHeaders {{"Title", Title}};
        return rv;
      }
    }

    public virtual string Title => Name;

    public void AddResource(IMediaResource res)
    {
      Resources.Add(res);
    }

    public virtual void Cleanup()
    {
      foreach (var m in merged) {
        m.Cleanup();
      }
      foreach (var f in Folders.ToList()) {
        f.Cleanup();
      }
      if (ChildCount != 0) {
        return;
      }
      var vp = Parent as VirtualFolder;
      vp?.ReleaseFolder(this);
    }

    public int CompareTo(IMediaItem other)
    {
      if (other == null) {
        return 1;
      }
      return comparer.Compare(Title, other.Title);
    }

    public bool Equals(IMediaItem other)
    {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }
      return Title.Equals(other.Title);
    }

    public bool RemoveResource(IMediaResource res)
    {
      return Resources.Remove(res);
    }

    public void Sort(IComparer<IMediaItem> sortComparer, bool descending)
    {
      foreach (var f in Folders) {
        f.Sort(sortComparer, descending);
      }
      Folders.Sort(sortComparer);
      Resources.Sort(sortComparer);
      if (descending) {
        Folders.Reverse();
        Resources.Reverse();
      }
    }

    public string ToComparableTitle()
    {
      return comparableTitle ?? (comparableTitle = Title.StemCompareBase());
    }

    public void AdoptFolder(IMediaFolder folder)
    {
      if (folder == null) {
        throw new ArgumentNullException(nameof(folder));
      }
      var vf = folder.Parent as VirtualFolder;
      vf?.ReleaseFolder(folder);
      folder.Parent = this;
      if (!Folders.Contains(folder)) {
        Folders.Add(folder);
      }
    }

    public void Merge(IMediaFolder folder)
    {
      if (folder == null) {
        throw new ArgumentNullException(nameof(folder));
      }
      merged.Add(folder);
      foreach (var item in folder.ChildItems) {
        AddResource(item);
      }
      foreach (var cf in folder.ChildFolders) {
        var ownFolder = (from f in Folders
                         where f is VirtualFolder && f.Title == cf.Title
                         select f as VirtualFolder
          ).FirstOrDefault();
        if (ownFolder == null) {
          ownFolder = new VirtualFolder(this, cf.Title, cf.Id);
          AdoptFolder(ownFolder);
        }
        ownFolder.Merge(cf);
      }
    }

    public void ReleaseFolder(IMediaFolder folder)
    {
      Folders.Remove(folder);
    }
  }
}
