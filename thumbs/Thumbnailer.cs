using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using NMaier.sdlna.Server;
using log4net;

namespace NMaier.sdlna.Thumbnails
{
  public class Thumbnailer : Logging
  {

    private static readonly LRUCache<string, byte[]> cache = new LRUCache<string, byte[]>(1 << 11);
    private static readonly Dictionary<MediaTypes, List<IThumbnailer>> thumbers = new Dictionary<MediaTypes, List<IThumbnailer>>();



    static Thumbnailer()
    {
      var types = Enum.GetValues(typeof(MediaTypes));
      foreach (MediaTypes i in types) {
        thumbers.Add(i, new List<IThumbnailer>());
      }
      var a = Assembly.GetExecutingAssembly();
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface("IThumbnailer") == null) {
          continue;
        }
        ConstructorInfo ctor = t.GetConstructor(new Type[] { });
        if (ctor == null) {
          continue;
        }
        try {
          var thumber = ctor.Invoke(new object[] { }) as IThumbnailer;
          if (thumber == null) {
            continue;
          }
          foreach (MediaTypes i in types) {
            if (thumber.Handling.HasFlag(i)) {
              thumbers[i].Add(thumber);
            }
          }
        }
        catch (Exception) { }
      }
    }




    public byte[] GetThumbnail(FileInfo file, int width, int height)
    {
      var ext = file.Extension.ToLower().Substring(1);
      var mediaType = DlnaMaps.Ext2Media[ext];

      var key = file.FullName;
      byte[] rv;
      if (GetThumbnailFromCache(ref key, width, height, out rv)) {
        return rv;
      }

      return GetThumbnailInternal(key, file, mediaType, width, height);
    }

    public byte[] GetThumbnail(string key, MediaTypes type, Stream stream, int width, int height)
    {
      byte[] rv;
      if (GetThumbnailFromCache(ref key, width, height, out rv)) {
        return rv;
      }
      return GetThumbnailInternal(key, stream, type, width, height);
    }

    private bool GetThumbnailFromCache(ref string key, int width, int height, out byte[] rv)
    {
      key = string.Format("{0}x{1} {2}", width, height, key);
      return cache.TryGetValue(key, out rv);
    }

    private byte[] GetThumbnailInternal(string key, object item, MediaTypes type, int width, int height)
    {
      var thumbnailers = thumbers[type];
      foreach (var thumber in thumbnailers) {
        try {
          using (var i = thumber.GetThumbnail(item, width, height)) {
            var rv = i.ToArray();
            cache.Add(key, rv);
            return rv;
          }
        }
        catch (Exception ex) {
          Debug(String.Format("{0} failed to thumbnail a resource", thumber.GetType()), ex);
          continue;
        }
      }
      throw new ArgumentException("Not a supported resource");
    }

    internal static Image ResizeImage(Image image, int width, int height)
    {
      if (image.Width <= width && image.Height <= height) {
        return image;
      }
      var nw = (float)image.Width;
      var nh = (float)image.Height;
      var factor = 1.0f;
      if (nw > nh) {
        factor = width / nw;
      }
      else {
        factor = height / nh;
      }
      nw = nw * factor;
      nh = nh * factor;

      var result = new Bitmap((int)nw, (int)nh);
      try {
        result.SetResolution(image.HorizontalResolution, image.VerticalResolution);
      }
      catch (Exception ex) {
        LogManager.GetLogger(typeof(Thumbnailer)).Debug("Failed to set resolution", ex);
      }
      using (Graphics graphics = Graphics.FromImage(result)) {
        graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighSpeed;
        graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
        graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighSpeed;
        graphics.DrawImage(image, 0, 0, result.Width, result.Height);
      }
      return result;
    }
  }
}
