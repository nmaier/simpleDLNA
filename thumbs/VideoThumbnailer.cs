using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using log4net;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.Thumbnails
{
  class VideoThumbnailer : Logging, IThumbnailer
  {

    public VideoThumbnailer()
    {
      if (Ffmpeg.FFMPEG == null) {
        throw new NotSupportedException("No ffmpeg available");
      }
    }



    public MediaTypes Handling
    {
      get { return MediaTypes.VIDEO; }
    }




    [MethodImpl(MethodImplOptions.Synchronized)]
    public MemoryStream GetThumbnail(object item, ref int width, ref int height)
    {
      if (item is Stream) {
        return GetThumbnailInternal(item as Stream, ref width, ref height);
      }
      if (item is FileInfo) {
        return GetThumbnailInternal(item as FileInfo, ref width, ref height);
      }
      throw new NotSupportedException();
    }

    private MemoryStream GetThumbnailFromProcess(Process p, ref int width, ref int height)
    {
      Debug("Starting ffmpeg");
      using (var req = new ReadRequest(p, p.StandardOutput.BaseStream)) {
        req.Read();

        if (!p.WaitForExit(20000)) {
          p.Kill();
          throw new ArgumentException("ffmpeg timed out");
        }
        if (p.ExitCode != 0) {
          throw new ArgumentException("ffmpeg does not understand the stream");
        }
        req.Finish();
        Debug("Done ffmpeg");

        using (var img = Image.FromStream(req.OutStream)) {
          using (var scaled = Thumbnailer.ResizeImage(img, ref width, ref height)) {
            var rv = new MemoryStream();
            scaled.Save(rv, ImageFormat.Jpeg);
            return rv;
          }
        }
      }
    }

    private MemoryStream GetThumbnailInternal(Stream stream, ref int width, ref int height)
    {
      using (var p = new Process()) {
        long pos = 20;
        try {
          var length = stream.Length;
          if (length < 10 * (1 << 20)) {
            pos = 5;
          }
          else if (length > 100 * (1 << 20)) {
            pos = 60;
          }
          else if (length > 50 * (1 << 20)) {
            pos = 60;
          }
        }
        catch (Exception) { }

        var sti = p.StartInfo;
#if !DEBUG
        sti.CreateNoWindow = true;
#endif
        sti.UseShellExecute = false;
        sti.FileName = Ffmpeg.FFMPEG.FullName;
        sti.Arguments = String.Format(
          "-ss {0} -i pipe: -an -frames:v 1 -f image2  pipe:",
          pos
          );
        sti.LoadUserProfile = false;
        sti.RedirectStandardInput = true;
        sti.RedirectStandardOutput = true;
        p.Start();

        using (var pump = new WriteRequest(p, stream, p.StandardInput.BaseStream)) {
          pump.Write();
          return GetThumbnailFromProcess(p, ref width, ref height);
        }

      }
    }

    private MemoryStream GetThumbnailInternal(FileInfo file, ref int width, ref int height)
    {
      using (var p = new Process()) {

        var sti = p.StartInfo;
#if !DEBUG
        sti.CreateNoWindow = true;
#endif
        sti.UseShellExecute = false;
        sti.FileName = Ffmpeg.FFMPEG.FullName;
        sti.Arguments = String.Format(
          "-ss {0} -i \"{1}\" -an -frames:v 1 -f image2  pipe:",
          IdentifyBestCapturePosition(file),
          file.FullName
          );
        sti.LoadUserProfile = false;
        sti.RedirectStandardOutput = true;
        p.Start();

        return GetThumbnailFromProcess(p, ref width, ref height);
      }
    }

    private long IdentifyBestCapturePosition(FileInfo file)
    {
      try {
        var dur = Ffmpeg.GetFileDuration(file);
        if (dur > 600) {
          return (long)(dur / 5.0);
        }
        return (long)(dur / 3.0);
      }
      catch (Exception) {
        // pass
      }
      var length = file.Length;
      if (length < 10 * (1 << 20)) {
        return 5;
      }
      if (length > 50 * (1 << 20)) {
        return 60;
      }
      if (length > 100 * (1 << 20)) {
        return 120;
      }
      if (length > 500 * (1 << 20)) {
        return 300;
      }
      if (length > 750 * (1 << 20)) {
        return 600;
      }
      return 20;
    }
  }
}
