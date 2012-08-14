using System.Collections.Generic;
using System.Linq;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  abstract class AbstractFolder : IFileServerFolder, IFileServerMediaItem
  {

    protected List<IFileServerFolder> childFolders;
    protected List<IFileServerResource> childItems;
    protected string id;
    protected IMediaFolder parent;
    protected readonly FileServer server;



    protected AbstractFolder(FileServer aServer, IMediaFolder aParent)
    {
      server = aServer;
      parent = aParent;
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
      get { return id; }
      set { id = value; }
    }

    public IMediaFolder Parent
    {
      get { return parent; }
      set { parent = value; }
    }

    abstract public string Path { get; }

    abstract public string Title { get; }




    public void AdoptItem(IFileServerMediaItem item)
    {
      if (item.Parent != null) {
        (item.Parent as IFileServerFolder).ReleaseItem(item);
      }
      item.Parent = this;
      if (item is IFileServerFolder) {
        childFolders.Add(item as IFileServerFolder);
      }
      else {
        childItems.Add(item as IFileServerResource);
      }
    }

    public int CompareTo(IMediaItem other)
    {
      return Title.CompareTo(other.Title);
    }

    public void ReleaseItem(IFileServerMediaItem item)
    {
      item.Parent = null;
      if (item is IFileServerFolder) {
        childFolders.Remove(item as IFileServerFolder);
      }
      else {
        childItems.Remove(item as IFileServerResource);
      }
    }

    public void Sort(IItemComparer comparer, bool descending)
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
