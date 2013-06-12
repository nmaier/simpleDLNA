namespace NMaier.SimpleDlna.Utilities
{
  public interface IRepositoryItem
  {
    string Description { get; }
    string Name { get; }

    void SetParameters(AttributeCollection parameters) ;
  }
}
