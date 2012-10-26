using System;
using System.Linq;
using System.Collections.Generic;
using NMaier.sdlna.FileMediaServer.Files;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  internal class VirtualFolder : BaseFolder
  {
    public VirtualFolder(FileServer server, BaseFolder aParent, string aName)
      : base(server, aParent)
    {
      Name = aName;
      childFolders = new List<BaseFolder>();
      childItems = new List<BaseFile>();
    }

    public VirtualFolder() : this(null, null, null) { }



    internal string Name
    {
      get;
      set;
    }

    private string path = null;
    public override string Path
    {
      get
      {
        if (string.IsNullOrEmpty(path)) {
          path = string.Format("{0}/virtual:{1}", Parent.Path, Name);
        }
        return path;
      }
    }

    public override string Title
    {
      get { return Name; }
    }




    public void Link(BaseFile r)
    {
      childItems.Add(r);
    }

    internal void AdoptChildren()
    {
      var children = (from c in childItems
                     where c.Parent != this
                     select c).ToList();
      childItems.Clear();
      foreach (var c in children) {
        AdoptItem(c);
      }
    }
  }
}
