using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.Thumbnails
{
  class ImageThumbnailer : IThumbnailer
  {

    public MediaTypes Handling
    {
      get { return MediaTypes.IMAGE; }
    }




    public MemoryStream GetThumbnail(object item, int width, int height)
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
        using (var scaled = Thumbnailer.ResizeImage(img, width, height)) {
          var rv = new MemoryStream();
          scaled.Save(rv, ImageFormat.Jpeg);
          return rv;
        }
      }
    }
  }
}
