using System.Collections.Generic;

namespace NMaier.sdlna.Server.Metadata
{
  public interface IMetaVideoItem : IMetaInfo, IMetaDescription, IMetaGenre, IMetaDuration, IMetaResolution {
    string MetaDirector { get; }
    IEnumerable<string> MetaActors { get; }
  }
}
