using System.Collections.Generic;
using NMaier.sdlna.FileMediaServer.Files;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  abstract class AbstractFolder : IFileServerFolder, IFileServerMediaItem
  {

    protected List<IFileServerFolder> childFolders;
    protected List<BaseFile> childItems;


    protected AbstractFolder(FileServer aServer, IFileServerFolder aParent)
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

    public IFileServerFolder Parent
    {
      get;
      set;
    }

    abstract public string Path { get; }

    internal FileServer Server
    {
      get;
      set;
    }

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
        childItems.Add(item as BaseFile);
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

    IMediaFolder IMediaItem.Parent
    {
      get { return Parent; }
    }

    FileServer IFileServerFolder.Server
    {
      get { return Server; }
    }
  }
}
