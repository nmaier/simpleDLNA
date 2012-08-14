using System;
using System.Collections.Generic;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class VirtualFolder : AbstractFolder
  {

    private readonly string name;
    private readonly string path = "virtual:" + Guid.NewGuid().ToString();



    public VirtualFolder(FileServer server, IMediaFolder aParent, string aName)
      : base(server, aParent)
    {
      name = aName;
      childFolders = new List<IFileServerFolder>();
      childItems = new List<IFileServerResource>();
    }



    public override string Path
    {
      get { return path; }
    }

    public override string Title
    {
      get { return name; }
    }
  }
}
