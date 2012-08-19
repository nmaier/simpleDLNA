
namespace NMaier.sdlna.Server.Metadata
{
  public interface IMetaImageItem : IMetaInfo, IMetaResolution, IMetaDescription
  {

    string MetaCreator { get; }
  }
}
