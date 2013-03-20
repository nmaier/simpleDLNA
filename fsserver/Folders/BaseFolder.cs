using System;
using System.Collections.Generic;
using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Files;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal abstract class BaseFolder : IFileServerMediaItem, IMediaFolder
  {
    protected List<BaseFolder> childFolders;

    protected List<BaseFile> childItems;


    protected BaseFolder(FileServer aServer, BaseFolder aParent)
    {
      Server = aServer;
      Parent = aParent;
    }


    IMediaFolder IMediaFolder.Parent
    {
      get
      {
        return Parent;
      }
    }


    internal FileServer Server
    {
      get;
      set;
    }


    public int ChildCount
    {
      get
      {
        return childFolders.Count + childItems.Count;
      }
    }
    public IEnumerable<IMediaFolder> ChildFolders
    {
      get
      {
        return childFolders;
      }
    }
    public IEnumerable<IMediaResource> ChildItems
    {
      get
      {
        return childItems;
      }
    }
    public string Id
    {
      get;
      set;
    }
    public BaseFolder Parent
    {
      get;
      set;
    }
    public abstract string Path { get; }
    public virtual IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Title", Title);
        return rv;
      }
    }
    public abstract string Title { get; }


    internal void AddFile(BaseFile file)
    {
      if (file == null) {
        throw new ArgumentNullException("file");
      }
      childItems.Add(file);
    }

    internal void RemoveFile(BaseFile file)
    {
      if (file == null) {
        throw new ArgumentNullException("file");
      }
      childItems.Remove(file);
    }


    public void AdoptFolder(BaseFolder folder)
    {
      if (folder.Parent != null) {
        folder.Parent.ReleaseFolder(folder);
      }
      folder.Parent = this;
      childFolders.Add(folder);
    }

    public virtual void Cleanup()
    {
      foreach (var f in childFolders) {
        f.Cleanup();
      }
      childFolders = (from f in childFolders
                      where f.ChildCount > 0
                      select f).ToList();
    }

    public int CompareTo(IMediaItem other)
    {
      return Title.ToLower().CompareTo(other.Title.ToLower());
    }

    public void ReleaseFolder(BaseFolder folder)
    {
      folder.Parent = null;
      childFolders.Remove(folder);
    }

    public void Sort(Comparers.IItemComparer comparer, bool descending)
    {
      foreach (var f in childFolders) {
        f.Sort(comparer, descending);
      }
      childFolders.Sort(comparer);
      childItems.Sort(comparer);
      if (descending) {
        childFolders.Reverse();
        childItems.Reverse();
      }
    }
  }
}
