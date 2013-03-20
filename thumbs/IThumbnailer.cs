using System;
using System.IO;
using NMaier.SimpleDlna.Server;

[assembly: CLSCompliant(true)]
namespace NMaier.SimpleDlna.Thumbnails
{
  internal interface IThumbnailer
  {
    DlnaMediaTypes Handling { get; }

    MemoryStream GetThumbnail(object item, ref int width, ref int height);
  }
}
