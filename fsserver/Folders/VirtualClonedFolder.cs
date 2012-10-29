using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class VirtualClonedFolder : VirtualFolder
  {

    private readonly MediaTypes types;



    public VirtualClonedFolder(FileServer server, IMediaFolder parent, string name, MediaTypes types = MediaTypes.AUDIO | MediaTypes.IMAGE | MediaTypes.VIDEO)
      : base(server, null, name)
    {
      this.types = types;
      Id = name;
      CloneFolder(this, parent);
      Cleanup();
    }

    public VirtualClonedFolder(BaseFolder parent, string name) : this(parent.Server, parent, name) { }

    public VirtualClonedFolder(BaseFolder parent) : this(parent.Server, parent, parent.Path) { }



    public override string Path { get { return Name; } }




    private void CloneFolder(VirtualFolder parent, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders) {
        var vf = new VirtualFolder(Server, parent, f.Title, f.Id);
        parent.AdoptFolder(vf);
        CloneFolder(vf, f as BaseFolder);
      }
      foreach (var i in folder.ChildItems) {
        if ((types & i.MediaType) == i.MediaType) {
          parent.AddFile(i as Files.BaseFile);
        }
      }
    }
  }
}
