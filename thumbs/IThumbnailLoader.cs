using NMaier.SimpleDlna.Server;
using System.IO;

namespace NMaier.SimpleDlna.Thumbnails
{
  internal interface IThumbnailLoader
  {
    DlnaMediaTypes Handling { get; }

    MemoryStream GetThumbnail(object item, ref int width, ref int height);
  }
}
