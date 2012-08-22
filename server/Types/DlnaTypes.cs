using System;
using System.Collections.Generic;

namespace NMaier.sdlna.Server
{
  public enum DlnaTypes
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
  public enum MediaTypes : uint
  {
    VIDEO = 1 << 0,
    IMAGE = 1 << 1,
    AUDIO = 1 << 2
  }

  public class DlnaMaps
  {

    private static string[] aacs = new string[] { "aac", "mp4a" };
    private static string[] avcs = new string[] { "avc", "mp4", "mov", "3gp", "3gpp", "flv" };
    private static string[] avis = new string[] { "avi", "divx", "xvid" };
    public static Dictionary<string, DlnaTypes> Ext2Dlna = new Dictionary<string, DlnaTypes>();
    public static Dictionary<string, MediaTypes> Ext2Media = new Dictionary<string, MediaTypes>();
    private static string[] jpgs = new string[] { "jpg", "jpe", "jpeg", "jif", "jfif" };
    public static Dictionary<MediaTypes, List<string>> Media2Ext = new Dictionary<MediaTypes, List<string>>();
    public static Dictionary<DlnaTypes, string> Mime = new Dictionary<DlnaTypes, string>();
    private static string[] mkvs = new string[] { "mkv", "matroska", "mk3d", "webm" };
    private static string[] mp3s = new string[] { "mp3", "mp3p", "mp3x", "mp3a", "mpa" };
    private static string[] mpgs = new string[] { "mpg", "mpe", "mpeg", "mpg2", "mpeg2", "ts", "vob", "m2v" };
    private static string[] ogas = new string[] { "ogg", "oga" };
    public static Dictionary<DlnaTypes, string> PN = new Dictionary<DlnaTypes, string>();



    static DlnaMaps()
    {
      PN.Add(DlnaTypes.MATROSKA, "DLNA.ORG_PN=MATROSKA");
      PN.Add(DlnaTypes.AVI, "DLNA.ORG_PN=AVI");
      PN.Add(DlnaTypes.MPEG, "DLNA.ORG_PN=MPEG1");
      PN.Add(DlnaTypes.JPEG, "DLNA.ORG_PN=JPEG");
      PN.Add(DlnaTypes.AVC, "DLNA.ORG_P=AVC_TS_HD");
      PN.Add(DlnaTypes.MP3, "DLNA.ORG_PN=MP3");
      PN.Add(DlnaTypes.AAC, "DLNA.ORG_PN=AAC");
      PN.Add(DlnaTypes.VORBIS, "DLNA.ORG_PN=OGG");

      Mime.Add(DlnaTypes.MATROSKA, "video/x-mkv");
      Mime.Add(DlnaTypes.AVI, "video/avi");
      Mime.Add(DlnaTypes.MPEG, "video/mpeg");
      Mime.Add(DlnaTypes.JPEG, "image/jpeg");
      Mime.Add(DlnaTypes.AVC, "video/mp4");
      Mime.Add(DlnaTypes.MP3, "audio/mpeg");
      Mime.Add(DlnaTypes.AAC, "audio/aac");
      Mime.Add(DlnaTypes.VORBIS, "audio/ogg");

      var e2d = new[] {
        new { t = DlnaTypes.MATROSKA, e = mkvs },
        new { t = DlnaTypes.AVI, e = avis },
        new { t = DlnaTypes.MPEG, e = mpgs },
        new { t = DlnaTypes.JPEG, e = jpgs },
        new { t = DlnaTypes.AVC, e = avcs },
        new { t = DlnaTypes.MP3, e = mp3s },
        new { t = DlnaTypes.AAC, e = aacs },
        new { t = DlnaTypes.VORBIS, e = ogas },
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
