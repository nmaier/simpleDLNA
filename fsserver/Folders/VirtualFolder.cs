using System;
using System.Collections.Generic;
using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Files;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class VirtualFolder : BaseFolder
  {

    private readonly string id;
    private string path = null;



    public VirtualFolder(FileServer server, BaseFolder parent, string name, string id)
      : base(server, parent)
    {
      this.id = id;
      Name = name;
      childFolders = new List<BaseFolder>();
      childItems = new List<BaseFile>();
    }

    public VirtualFolder(FileServer server, BaseFolder parent, string name)
      : this(server, parent, name, name)
    {
    }

    public VirtualFolder() : this(null, null, null) { }



    internal string Name
    {
      get;
      set;
    }

    public override string Path
    {
      get
      {
        if (string.IsNullOrEmpty(path)) {
          var p = string.IsNullOrEmpty(id) ? Name : id;
          if (Parent != null) {
            path = string.Format("{0}/:{1}", Parent.Path, p);
          }
          else {
            path = p;
          }
        }
        return path;
      }
    }

    public override string Title
    {
      get { return Name; }
    }




    internal void Merge(BaseFolder folder)
    {
      if (folder == null) {
        throw new ArgumentNullException("folder");
      }
      foreach (var item in folder.ChildItems) {
        AddFile(item as Files.BaseFile);
      }
      foreach (var cf in folder.ChildFolders) {
        VirtualFolder ownFolder = (from f in childFolders
                                   where f is VirtualFolder && f.Title == cf.Title
                                   select f as VirtualFolder
                                   ).DefaultIfEmpty(null).FirstOrDefault();
        if (ownFolder == null) {
          ownFolder = new VirtualFolder(Server, this, cf.Title);
          childFolders.Add(ownFolder);
        }
        ownFolder.Merge(cf as BaseFolder);
      }
    }
  }
}
