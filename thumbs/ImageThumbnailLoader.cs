using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.Thumbnails
{
  internal sealed class ImageThumbnailLoader : IThumbnailLoader
  {
    public DlnaMediaTypes Handling => DlnaMediaTypes.Image;

    public MemoryStream GetThumbnail(object item, ref int width,
      ref int height)
    {
      Image img;
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
        using (var scaled = ThumbnailMaker.ResizeImage(
          img, width, height, ThumbnailMakerBorder.Borderless)) {
          width = scaled.Width;
          height = scaled.Height;
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
