using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class VirtualClonedFolder : VirtualFolder
  {
    private readonly BaseFolder clone;

    private readonly DlnaMediaTypes types;


    public VirtualClonedFolder(BaseFolder parent)
      : this(parent.Server, parent, parent.Path)
    {
    }
    public VirtualClonedFolder(BaseFolder parent, string name)
      : this(parent.Server, parent, name)
    {
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public VirtualClonedFolder(FileServer server, BaseFolder parent, string name, DlnaMediaTypes types = DlnaMediaTypes.Audio | DlnaMediaTypes.Image | DlnaMediaTypes.Video)
      : base(server, null, name)
    {
      this.types = types;
      Id = name;
      clone = parent;
      CloneFolder(this, parent);
      Cleanup();
    }


    public override string Path
    {
      get
      {
        return Name;
      }
    }


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


    public override void Cleanup()
    {
      base.Cleanup();
      clone.Cleanup();
    }
  }
}
