using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.Thumbnails
{
  internal sealed class ImageThumbnailer : IThumbnailer
  {

    public MediaTypes Handling
    {
      get { return MediaTypes.IMAGE; }
    }




    public MemoryStream GetThumbnail(object item, ref int width, ref int height)
    {
      Image img = null;
      if (item is Stream) {
        img = Image.FromStream(item as Stream);
      }
      else if (item is FileInfo) {
        img = Image.FromFile((item as FileInfo).FullName);
      }
      else {
        throw new NotSupportedException();
      }
      using (img) {
        using (var scaled = Thumbnailer.ResizeImage(img, ref width, ref height)) {
          var rv = new MemoryStream();
          scaled.Save(rv, ImageFormat.Jpeg);
          return rv;
        }
      }
    }
  }
}
