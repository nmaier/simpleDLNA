namespace NMaier.SimpleDlna.Server.Comparers
{
  internal abstract class BaseComparer : IItemComparer
  {
    public abstract string Description { get; }
    public abstract string Name { get; }


    public abstract int Compare(IMediaItem x, IMediaItem y);

    public override string ToString()
    {
      return string.Format("{0} - {1}", Name, Description);
    }
  }
}
