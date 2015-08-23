using NMaier.SimpleDlna.Server;
using System;
using System.Collections.Generic;

namespace tests.Mocks
{
  public class MediaFolder : IMediaFolder
    {
      public int ChildCount
      {
        get
        {
          throw new NotImplementedException();
        }
      }

      public List<IMediaFolder> AccessorChildFolders = new List<IMediaFolder>();

      public IEnumerable<IMediaFolder> ChildFolders
      {
        get
        {
          return AccessorChildFolders;
        }
      }

      public List<IMediaResource> AccessorChildItems = new List<IMediaResource>();

      public IEnumerable<IMediaResource> ChildItems
      {
        get
        {
          return AccessorChildItems;
        }
      }

      public string Id { get; set; }

      public IMediaFolder Parent { get; set; }

      public string Path { get; set; }

      public IHeaders Properties { get; set; }

      public string Title { get; set; }

    public int FullChildCount
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public void AddResource(IMediaResource res)
      {
        throw new NotImplementedException();
      }

      public void Cleanup()
      {
      }

      public int CompareTo(IMediaItem other)
      {
        throw new NotImplementedException();
      }

      public bool Equals(IMediaItem other)
      {
        throw new NotImplementedException();
      }

      public void RemoveResource(IMediaResource res)
      {
        throw new NotImplementedException();
      }

      public void Sort(IComparer<IMediaItem> comparer, bool descending)
      {
      }

      public string ToComparableTitle()
      {
        throw new NotImplementedException();
      }
    }
}
