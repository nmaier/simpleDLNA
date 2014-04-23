using System;
using System.Collections.Generic;
using System.Linq;

namespace NMaier.SimpleDlna.Server
{
  public static class DlnaMaps
  {
    internal static readonly string DefaultStreaming = FlagsToString(
      DlnaFlags.StreamingTransferMode |
      DlnaFlags.BackgroundTransferMode |
      DlnaFlags.ConnectionStall |
      DlnaFlags.ByteBasedSeek |
      DlnaFlags.DlnaV15
      );

    internal static readonly string DefaultInteractive = FlagsToString(
      DlnaFlags.InteractiveTransferMode |
      DlnaFlags.BackgroundTransferMode |
      DlnaFlags.ConnectionStall |
      DlnaFlags.ByteBasedSeek |
      DlnaFlags.DlnaV15
      );

    private static readonly string[] aacs =
      new string[] { "aac", "mp4a", "m4a" };

    private static readonly string[] avcs =
      new string[] { "avc", "mp4", "m4v", "mov", "3gp", "3gpp", "flv" };

    private static readonly string[] avis =
      new string[] { "avi", "divx", "xvid" };

    public static readonly Dictionary<DlnaMime, List<string>> Dlna2Ext =
      new Dictionary<DlnaMime, List<string>>();

    public static readonly Dictionary<string, DlnaMime> Ext2Dlna =
      new Dictionary<string, DlnaMime>();

    public static readonly Dictionary<string, DlnaMediaTypes> Ext2Media =
      new Dictionary<string, DlnaMediaTypes>();

    private static readonly string[] jpgs =
      new string[] { "jpg", "jpe", "jpeg", "jif", "jfif" };

    public static readonly Dictionary<DlnaMediaTypes, List<string>> Media2Ext =
      new Dictionary<DlnaMediaTypes, List<string>>();

    public static readonly Dictionary<DlnaMime, string> Mime = new Dictionary<DlnaMime, string>() {
        { DlnaMime.MATROSKA, "video/x-mkv" },
        { DlnaMime.AVI, "video/avi" },
        { DlnaMime.MPEG, "video/mpeg" },
        { DlnaMime.JPEG, "image/jpeg" },
        { DlnaMime.AVC, "video/mp4" },
        { DlnaMime.MP3, "audio/mpeg" },
        { DlnaMime.AAC, "audio/aac" },
        { DlnaMime.VORBIS, "audio/ogg" },
        { DlnaMime.WMV, "video/x-ms-wmv" },
        { DlnaMime.SRT, "smi/caption" }
        };

    private static readonly string[] mkvs =
      new string[] { "mkv", "matroska", "mk3d", "webm" };

    private static readonly string[] mp3s =
      new string[] { "mp3", "mp3p", "mp3x", "mp3a", "mpa" };

    private static readonly string[] mpgs =
      new string[] { "mpg", "mpe", "mpeg", "mpg2", "mpeg2", "ts", "vob", "m2v" };

    private static readonly string[] ogas =
      new string[] { "ogg", "oga" };

    public static readonly Dictionary<DlnaMime, string> PN =
      new Dictionary<DlnaMime, string>() {
        { DlnaMime.MATROSKA, "DLNA.ORG_PN=MATROSKA" },
        { DlnaMime.AVI, "DLNA.ORG_PN=AVI" },
        { DlnaMime.MPEG, "DLNA.ORG_PN=MPEG1" },
        { DlnaMime.JPEG, "DLNA.ORG_PN=JPEG" },
        { DlnaMime.AVC, "DLNA.ORG_PN=AVC_MP4_MP_SD_AAC_MULT5" },
        { DlnaMime.MP3, "DLNA.ORG_PN=MP3" },
        { DlnaMime.AAC, "DLNA.ORG_PN=AAC" },
        { DlnaMime.VORBIS, "DLNA.ORG_PN=OGG" },
        { DlnaMime.WMV, "DLNA.ORG_PN=WMVHIGH_FULL" },
        { DlnaMime.SRT, "DLNA.ORG_PN=SRT" }
        };

    private static readonly string[] wmvs =
      new string[] { "wmv", "asf", "wma", "wmf" };

    static DlnaMaps()
    {
      var e2d = new[] {
        new
        { t = DlnaMime.MATROSKA, e = mkvs },
        new
        { t = DlnaMime.AVI, e = avis },
        new
        { t = DlnaMime.MPEG, e = mpgs },
        new
        { t = DlnaMime.JPEG, e = jpgs },
        new
        { t = DlnaMime.AVC, e = avcs },
        new
        { t = DlnaMime.MP3, e = mp3s },
        new
        { t = DlnaMime.AAC, e = aacs },
        new
        { t = DlnaMime.VORBIS, e = ogas },
        new
        { t = DlnaMime.WMV, e = wmvs }
      };

      foreach (var i in e2d) {
        var t = i.t;
        foreach (var e in i.e) {
          Ext2Dlna.Add(e.ToUpperInvariant(), t);
        }
        Dlna2Ext.Add(i.t, new List<string>(i.e));
      }

      InitMedia(
        new string[][] { avis, mkvs, mpgs, avcs, wmvs },
        DlnaMediaTypes.Video);
      InitMedia(
        new string[][] { jpgs },
        DlnaMediaTypes.Image);
      InitMedia(
        new string[][] { mp3s, aacs, ogas },
        DlnaMediaTypes.Audio);
    }

    private static void InitMedia(string[][] k, DlnaMediaTypes t)
    {
      foreach (var i in k) {
        var e = (from ext in i select ext.ToUpperInvariant()).ToList();
        try {
          Media2Ext.Add(t, e);
        }
        catch (ArgumentException) {
          Media2Ext[t].AddRange(e);
        }
        foreach (var ext in e) {
          Ext2Media.Add(ext.ToUpperInvariant(), t);
        }
      }
    }

    internal static string FlagsToString(DlnaFlags flags)
    {
      return string.Format("{0:X8}{1:D24}", (ulong)flags, 0);
    }
  }
}
