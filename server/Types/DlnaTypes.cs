using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server
{
  public enum DlnaType
  {
    MATROSKA,
    AVI,
    MPEG,
    JPEG,
    AVC,
    MP3,
    AAC,
    VORBIS
  }

  [Flags]
  public enum MediaTypes : byte
  {
    VIDEO = 1 << 0,
    IMAGE = 1 << 1,
    AUDIO = 1 << 2
  }

  public static class DlnaMaps
  {

    private static readonly string[] aacs = new string[] { "aac", "mp4a" };
    private static readonly string[] avcs = new string[] { "avc", "mp4", "mov", "3gp", "3gpp", "flv" };
    private static readonly string[] avis = new string[] { "avi", "divx", "xvid" };
    public static readonly Dictionary<DlnaType, List<string>> Dlna2Ext = new Dictionary<DlnaType, List<string>>();
    public static readonly Dictionary<string, DlnaType> Ext2Dlna = new Dictionary<string, DlnaType>();
    public static readonly Dictionary<string, MediaTypes> Ext2Media = new Dictionary<string, MediaTypes>();
    private static readonly string[] jpgs = new string[] { "jpg", "jpe", "jpeg", "jif", "jfif" };
    public static readonly Dictionary<MediaTypes, List<string>> Media2Ext = new Dictionary<MediaTypes, List<string>>();
    public static readonly Dictionary<DlnaType, string> Mime = new Dictionary<DlnaType, string>() {
      { DlnaType.MATROSKA, "video/x-mkv"},
      { DlnaType.AVI, "video/avi"},
      { DlnaType.MPEG, "video/mpeg"},
      { DlnaType.JPEG, "image/jpeg"},
      { DlnaType.AVC, "video/mp4"},
      { DlnaType.MP3, "audio/mpeg"},
      { DlnaType.AAC, "audio/aac"},
      { DlnaType.VORBIS, "audio/ogg"},
    };
    private static readonly string[] mkvs = new string[] { "mkv", "matroska", "mk3d", "webm" };
    private static readonly string[] mp3s = new string[] { "mp3", "mp3p", "mp3x", "mp3a", "mpa" };
    private static readonly string[] mpgs = new string[] { "mpg", "mpe", "mpeg", "mpg2", "mpeg2", "ts", "vob", "m2v" };
    private static readonly string[] ogas = new string[] { "ogg", "oga" };
    public static readonly Dictionary<DlnaType, string> PN = new Dictionary<DlnaType, string>() {
      { DlnaType.MATROSKA, "DLNA.ORG_PN=MATROSKA" },
      { DlnaType.AVI, "DLNA.ORG_PN=AVI" },
      { DlnaType.MPEG, "DLNA.ORG_PN=MPEG1" },
      { DlnaType.JPEG, "DLNA.ORG_PN=JPEG" },
      { DlnaType.AVC, "DLNA.ORG_P=AVC_TS_HD" },
      { DlnaType.MP3, "DLNA.ORG_PN=MP3" },
      { DlnaType.AAC, "DLNA.ORG_PN=AAC" },
      { DlnaType.VORBIS, "DLNA.ORG_PN=OGG" },
    };



    static DlnaMaps()
    {
      var e2d = new[] {
        new { t = DlnaType.MATROSKA, e = mkvs },
        new { t = DlnaType.AVI, e = avis },
        new { t = DlnaType.MPEG, e = mpgs },
        new { t = DlnaType.JPEG, e = jpgs },
        new { t = DlnaType.AVC, e = avcs },
        new { t = DlnaType.MP3, e = mp3s },
        new { t = DlnaType.AAC, e = aacs },
        new { t = DlnaType.VORBIS, e = ogas },
      };

      foreach (var i in e2d) {
        var t = i.t;
        foreach (var e in i.e) {
          Ext2Dlna.Add(e, t);
        }
        Dlna2Ext.Add(i.t, new List<string>(i.e));
      }

      InitMedia(new string[][] { avis, mkvs, mpgs, avcs }, MediaTypes.VIDEO);
      InitMedia(new string[][] { jpgs }, MediaTypes.IMAGE);
      InitMedia(new string[][] { mp3s, aacs, ogas }, MediaTypes.AUDIO);
    }




    private static void InitMedia(string[][] k, MediaTypes t)
    {
      foreach (var i in k) {
        var e = new List<string>(i);
        try {
          Media2Ext.Add(t, e);
        }
        catch (ArgumentException) {
          Media2Ext[t].AddRange(i);
        }
        foreach (var ext in e) {
          Ext2Media.Add(ext, t);
        }
      }
    }
  }
}
