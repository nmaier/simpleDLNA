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
          path = string.Format("{0}/:{1}", Parent.Path, string.IsNullOrEmpty(id) ? Name : id);
        }
        return path;
      }
    }

    public override string Title
    {
      get { return Name; }
    }




    public void LinkFile(BaseFile file)
    {
      childItems.Add(file);
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
