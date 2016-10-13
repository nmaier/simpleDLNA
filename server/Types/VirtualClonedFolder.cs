namespace NMaier.SimpleDlna.Server
{
  public sealed class VirtualClonedFolder : VirtualFolder
  {
    private readonly IMediaFolder clone;

    private readonly DlnaMediaTypes types;

    private VirtualClonedFolder(IMediaFolder parent, string name, string id,
      DlnaMediaTypes types)
      : base(parent, name, id)
    {
      this.types = types;
      Id = id;
      clone = parent;
      CloneFolder(this, parent);
      Cleanup();
    }

    public VirtualClonedFolder(IMediaFolder parent)
      : this(parent, parent.Id, parent.Id, DlnaMediaTypes.All)
    {
    }

    public VirtualClonedFolder(IMediaFolder parent, string name)
      : this(parent, name, name, DlnaMediaTypes.All)
    {
    }

    public VirtualClonedFolder(IMediaFolder parent, string name,
      DlnaMediaTypes types)
      : this(parent, name, name, types)
    {
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
