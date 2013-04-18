namespace NMaier.SimpleDlna.Server
{
  public class VirtualClonedFolder : VirtualFolder
  {
    private readonly IMediaFolder clone;

    private readonly DlnaMediaTypes types;


    public VirtualClonedFolder(IMediaFolder parent)
      : this(parent, parent.Id, DlnaMediaTypes.Audio | DlnaMediaTypes.Image | DlnaMediaTypes.Video)
    {
    }
    public VirtualClonedFolder(IMediaFolder parent, string name)
      : this(parent, name, DlnaMediaTypes.Audio | DlnaMediaTypes.Image | DlnaMediaTypes.Video)
    {
    }
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
    public VirtualClonedFolder(IMediaFolder parent, string name, DlnaMediaTypes types)
      : base(parent, name, name)
    {
      this.types = types;
      Id = name;
      clone = parent;
      CloneFolder(this, parent);
      Cleanup();
    }


    private void CloneFolder(VirtualFolder parent, IMediaFolder folder)
    {
      foreach (var f in folder.ChildFolders) {
        var vf = new VirtualFolder(parent, f.Title, f.Id);
        parent.AdoptFolder(vf);
        CloneFolder(vf, f);
      }
      foreach (var i in folder.ChildItems) {
        if ((types & i.MediaType) == i.MediaType) {
          parent.AddResource(i);
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
