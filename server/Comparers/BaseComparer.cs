using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal abstract class BaseComparer : IItemComparer
  {
    public abstract string Description { get; }

    public abstract string Name { get; }

    public abstract int Compare(IMediaItem x, IMediaItem y);

    public override string ToString()
    {
      return $"{Name} - {Description}";
    }
  }
}
