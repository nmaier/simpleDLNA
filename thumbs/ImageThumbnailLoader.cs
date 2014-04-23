using NMaier.SimpleDlna.Server;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

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
      var stream = item as Stream;
      if (stream != null) {
        img = Image.FromStream(stream);
      }
      else {
        var fi = item as FileInfo;
        if (fi != null) {
          img = Image.FromFile(fi.FullName);
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
