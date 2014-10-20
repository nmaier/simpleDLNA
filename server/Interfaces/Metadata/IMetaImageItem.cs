namespace NMaier.SimpleDlna.Server.Metadata
{
  public interface IMetaImageItem
    : IMetaInfo, IMetaResolution, IMetaDescription
  {
    string MetaCreator { get; }
  }
}
