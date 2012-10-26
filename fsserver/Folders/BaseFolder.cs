using System.Collections.Generic;
using System.Linq;
using NMaier.sdlna.FileMediaServer.Files;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  abstract class BaseFolder : IFileServerMediaItem, IMediaFolder
  {

    protected List<BaseFolder> childFolders;
    protected List<BaseFile> childItems;



    protected BaseFolder(FileServer aServer, BaseFolder aParent)
    {
      Server = aServer;
      Parent = aParent;
    }



    public uint ChildCount
    {
      get { return (uint)(childFolders.Count + childItems.Count); }
    }

    public IEnumerable<IMediaFolder> ChildFolders
    {
      get { return childFolders; }
    }

    public IEnumerable<IMediaResource> ChildItems
    {
      get { return childItems; }
    }

    public string ID
    {
      get;
      set;
    }

    IMediaFolder IMediaItem.Parent
    {
      get { return Parent; }
    }

    public BaseFolder Parent
    {
      get;
      set;
    }

    abstract public string Path { get; }

    public virtual IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Title", Title);
        return rv;
      }
    }

    internal FileServer Server
    {
      get;
      set;
    }

    abstract public string Title { get; }




    public void AdoptItem(IFileServerMediaItem item)
    {
      if (item.Parent != null) {
        (item.Parent as BaseFolder).ReleaseItem(item);
      }
      item.Parent = this;
      if (item is BaseFolder) {
        childFolders.Add(item as BaseFolder);
      }
      else {
        childItems.Add(item as BaseFile);
      }
    }

    public int CompareTo(IMediaItem other)
    {
      return Title.ToLower().CompareTo(other.Title.ToLower());
    }

    public void Cleanup()
    {
      foreach (var f in childFolders) {
        f.Cleanup();
      }
      childFolders = (from f in childFolders
                      where f.ChildCount > 0
                      select f).ToList();
    }

    public void ReleaseItem(IFileServerMediaItem item)
    {
      item.Parent = null;
      if (item is BaseFolder) {
        childFolders.Remove(item as BaseFolder);
      }
      else {
        childItems.Remove(item as BaseFile);
      }
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
