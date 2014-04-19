using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using log4net;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Thumbnails
{
  using Drawing2D = System.Drawing.Drawing2D;

  public sealed class ThumbnailMaker : Logging
  {
    private static readonly LeastRecentlyUsedDictionary<string, CacheItem> cache =
      new LeastRecentlyUsedDictionary<string, CacheItem>(1 << 11);

    private static readonly Dictionary<DlnaMediaTypes, List<IThumbnailLoader>> thumbers =
      BuildThumbnailers();

    private static Dictionary<DlnaMediaTypes, List<IThumbnailLoader>> BuildThumbnailers()
    {
      var thumbers = new Dictionary<DlnaMediaTypes, List<IThumbnailLoader>>();
      var types = Enum.GetValues(typeof(DlnaMediaTypes));
      foreach (DlnaMediaTypes i in types) {
        thumbers.Add(i, new List<IThumbnailLoader>());
      }
      var a = Assembly.GetExecutingAssembly();
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface("IThumbnailLoader") == null) {
          continue;
        }
        var ctor = t.GetConstructor(new Type[] { });
        if (ctor == null) {
          continue;
        }
        var thumber = ctor.Invoke(new object[] { }) as IThumbnailLoader;
        if (thumber == null) {
          continue;
        }
        foreach (DlnaMediaTypes i in types) {
          if (thumber.Handling.HasFlag(i)) {
            thumbers[i].Add(thumber);
          }
        }
      }
      return thumbers;
    }

    private static bool GetThumbnailFromCache(ref string key, ref int width, ref int height, out byte[] rv)
    {
      key = string.Format("{0}x{1} {2}", width, height, key);
      CacheItem ci;
      lock (cache) {
        if (cache.TryGetValue(key, out ci)) {
          rv = ci.Data;
          width = ci.Width;
          height = ci.Height;
          return true;
        }
      }
      rv = null;
      return false;
    }

    private byte[] GetThumbnailInternal(string key, object item, DlnaMediaTypes type, ref int width, ref int height)
    {
      var thumbnailers = thumbers[type];
      var rw = width;
      var rh = height;
      foreach (var thumber in thumbnailers) {
        try {
          using (var i = thumber.GetThumbnail(item, ref width, ref height)) {
            var rv = i.ToArray();
            lock (cache) {
              cache[key] = new CacheItem(rv, rw, rh);
            }
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

    internal static Image ResizeImage(Image image, ref int width, ref int height)
    {
      if (image.Width <= width && image.Height <= height) {
        foreach (var p in image.PropertyIdList.Clone() as int[]) {
          image.RemovePropertyItem(p);
        }
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
        try {
          result.SetResolution(image.HorizontalResolution, image.VerticalResolution);
        }
        catch (Exception ex) {
          LogManager.GetLogger(typeof(ThumbnailMaker)).Debug("Failed to set resolution", ex);
        }
        using (var graphics = Graphics.FromImage(result)) {
          if (result.Width > image.Width && result.Height > image.Height) {
            graphics.CompositingQuality = Drawing2D.CompositingQuality.HighQuality;
            graphics.InterpolationMode = Drawing2D.InterpolationMode.High;
          }
          else {
            graphics.CompositingQuality = Drawing2D.CompositingQuality.HighSpeed;
            graphics.InterpolationMode = Drawing2D.InterpolationMode.Bicubic;
          }
          graphics.SmoothingMode = Drawing2D.SmoothingMode.HighSpeed;
          graphics.DrawImage(image, 0, 0, result.Width, result.Height);
          width = result.Width;
          height = result.Height;
        }
        return result;
      }
      catch (Exception) {
        result.Dispose();
        throw;
      }
    }

    public IThumbnail GetThumbnail(FileSystemInfo file, int width, int height)
    {
      if (file == null) {
        throw new ArgumentNullException("file");
      }
      var ext = file.Extension.ToLower().Substring(1);
      var mediaType = DlnaMaps.Ext2Media[ext];

      var key = file.FullName;
      byte[] rv;
      if (GetThumbnailFromCache(ref key, ref width, ref height, out rv)) {
        return new Thumbnail(width, height, rv);
        ;
      }

      rv = GetThumbnailInternal(key, file, mediaType, ref width, ref height);
      return new Thumbnail(width, height, rv);
    }

    public IThumbnail GetThumbnail(string key, DlnaMediaTypes type, Stream stream, int width, int height)
    {
      byte[] rv;
      if (GetThumbnailFromCache(ref key, ref width, ref height, out rv)) {
        return new Thumbnail(width, height, rv);
      }
      rv = GetThumbnailInternal(key, stream, type, ref width, ref height);
      return new Thumbnail(width, height, rv);
    }

    private class CacheItem
    {
      public readonly byte[] Data;

      public readonly int Height;

      public readonly int Width;

      public CacheItem(byte[] aData, int aWidth, int aHeight)
      {
        Data = aData;
        Width = aWidth;
        Height = aHeight;
      }
    }
  }
}
