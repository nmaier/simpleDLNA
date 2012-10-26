using System.IO;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.Thumbnails
{
  internal interface IThumbnailer
  {

    MediaTypes Handling { get; }



    MemoryStream GetThumbnail(object item, ref int width, ref int height);
  }
}
