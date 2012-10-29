using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.Thumbnails
{
  internal interface IThumbnailer
  {

    MediaTypes Handling { get; }



    MemoryStream GetThumbnail(object item, ref int width, ref int height);
  }
}
