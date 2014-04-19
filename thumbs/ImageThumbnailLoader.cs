using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.Thumbnails
{
  internal sealed class ImageThumbnailLoader : IThumbnailLoader
  {
    public DlnaMediaTypes Handling
    {
      get
      {
        return DlnaMediaTypes.Image;
      }
    }

    public MemoryStream GetThumbnail(object item, ref int width, ref int height)
    {
      Image img = null;
      if (item is Stream) {
        img = Image.FromStream(item as Stream);
      }
      else {
        if (item is FileInfo) {
          img = Image.FromFile((item as FileInfo).FullName);
        }
        else {
          throw new NotSupportedException();
        }
      }
      using (img) {
        using (var scaled = ThumbnailMaker.ResizeImage(img, ref width, ref height)) {
          var rv = new MemoryStream();
          try {
            scaled.Save(rv, ImageFormat.Jpeg);
            return rv;
          }
          catch (Exception) {
            rv.Dispose();
            throw;
          }
        }
      }
    }
  }
}
