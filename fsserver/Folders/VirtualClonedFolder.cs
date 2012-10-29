using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class VirtualClonedFolder : VirtualFolder
  {

    private readonly MediaTypes types;



    public VirtualClonedFolder(FileServer server, IMediaFolder parent, string name, MediaTypes types)
      : base(server, null, name)
    {
      this.types = types;
      Id = name;
      CloneFolder(this, parent);
      Cleanup();
    }



    public override string Path { get { return Name; } }




    private void CloneFolder(VirtualFolder parent, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders) {
        var vf = new VirtualFolder(Server, parent, f.Title, f.Id);
        parent.AdoptItem(vf);
        CloneFolder(vf, f as BaseFolder);
      }
      foreach (var i in folder.ChildItems) {
        if ((types & i.MediaType) == i.MediaType) {
          parent.LinkFile(i as Files.BaseFile);
        }
      }
    }
  }
}
