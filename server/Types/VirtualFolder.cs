using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NMaier.SimpleDlna.Server
{
  public class VirtualFolder : IMediaFolder, ITitleComparable
  {
    private string comparableTitle;

    private readonly List<IMediaFolder> merged = new List<IMediaFolder>();

    protected List<IMediaFolder> folders = new List<IMediaFolder>();

    protected List<IMediaResource> resources = new List<IMediaResource>();

    private static readonly StringComparer comparer =
      new NaturalStringComparer(true);

    private string path;

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
      get
      {
        return folders.SelectMany(f => (f as VirtualFolder).AllItems).
          Concat(resources);
      }
    }

    public int ChildCount
    {
      get
      {
        return folders.Count + resources.Count;
      }
    }

    public int FullChildCount
    {
      get
      {
        return resources.Count + (from f in folders select f.FullChildCount).Sum();
      }
    }

    public IEnumerable<IMediaFolder> ChildFolders
    {
      get
      {
        return folders;
      }
    }

    public IEnumerable<IMediaResource> ChildItems
    {
      get
      {
        return resources;
      }
    }

    public string Id { get; set; }

    public string Name { get; set; }

    public IMediaFolder Parent
    {
      get;
      set;
    }

    public virtual string Path
    {
      get
      {
        if (!string.IsNullOrEmpty(path)) {
          return path;
        }
        var p = string.IsNullOrEmpty(Id) ? Name : Id;
        if (Parent != null) {
          var vp = Parent as VirtualFolder;
          if (vp != null) {
            path = string.Format("{0}/:{1}", vp.Path, p);
          }
          else {
            path = string.Format("{0}/:{1}", Parent.Id, p);
          }
        }
        else {
          path = p;
        }
        return path;
      }
    }

    public IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Title", Title);
        return rv;
      }
    }

    public virtual string Title
    {
      get
      {
        return Name;
      }
    }

    public void AddResource(IMediaResource res)
    {
      resources.Add(res);
    }

    public void AdoptFolder(IMediaFolder folder)
    {
      if (folder == null) {
        throw new ArgumentNullException("folder");
      }
      var vf = folder.Parent as VirtualFolder;
      if (vf != null) {
        vf.ReleaseFolder(folder);
      }
      folder.Parent = this;
      if (!folders.Contains(folder)) {
        folders.Add(folder);
      }
    }

    public virtual void Cleanup()
    {
      foreach (var m in merged) {
        m.Cleanup();
      }
      foreach (var f in folders.ToList()) {
        f.Cleanup();
      }
      if (ChildCount != 0) {
        return;
      }
      var vp = Parent as VirtualFolder;
      if (vp != null) {
        vp.ReleaseFolder(this);
      }
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
        throw new ArgumentNullException("other");
      }
      return Title.Equals(other.Title);
    }

    public void Merge(IMediaFolder folder)
    {
      if (folder == null) {
        throw new ArgumentNullException("folder");
      }
      merged.Add(folder);
      foreach (var item in folder.ChildItems) {
        AddResource(item);
      }
      foreach (var cf in folder.ChildFolders) {
        var ownFolder = (from f in folders
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
      folders.Remove(folder);
    }

    public bool RemoveResource(IMediaResource res)
    {
      return resources.Remove(res);
    }

    public void Sort(IComparer<IMediaItem> comparer, bool descending)
    {
      foreach (var f in folders) {
        f.Sort(comparer, descending);
      }
      folders.Sort(comparer);
      resources.Sort(comparer);
      if (descending) {
        folders.Reverse();
        resources.Reverse();
      }
    }

    public string ToComparableTitle()
    {
      if (comparableTitle == null) {
        comparableTitle = Title.StemCompareBase();
      }
      return comparableTitle;
    }
  }
}
