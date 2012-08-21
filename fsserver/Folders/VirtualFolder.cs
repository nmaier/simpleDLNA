using System;
using System.Collections.Generic;
using NMaier.sdlna.FileMediaServer.Files;

namespace NMaier.sdlna.FileMediaServer.Folders
{
  class VirtualFolder : BaseFolder
  {

    private readonly string path = "virtual:" + Guid.NewGuid().ToString();



    public VirtualFolder(FileServer server, BaseFolder aParent, string aName)
      : base(server, aParent)
    {
      Name = aName;
      childFolders = new List<BaseFolder>();
      childItems = new List<BaseFile>();
    }

    public VirtualFolder() : this(null, null, null) { }



    internal string Name {
      get;
      set;
    }

    public override string Path
    {
      get { return path; }
    }

    public override string Title
    {
      get { return Name; }
    }




    public void Link(BaseFile r)
    {
      childItems.Add(r);
    }
  }
}
