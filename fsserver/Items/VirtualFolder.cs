using System;
using System.Collections.Generic;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class VirtualFolder : AbstractFolder
  {

    private readonly string path = "virtual:" + Guid.NewGuid().ToString();



    public VirtualFolder(FileServer server, IFileServerFolder aParent, string aName)
      : base(server, aParent)
    {
      Name = aName;
      childFolders = new List<IFileServerFolder>();
      childItems = new List<IFileServerResource>();
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




    public void Link(IFileServerResource r)
    {
      childItems.Add(r);
    }
  }
}
