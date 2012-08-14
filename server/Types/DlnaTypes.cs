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
    AVC
  }

  [Flags]
  public enum MediaTypes : uint
  {
    VIDEO = 1<<0,
    IMAGE = 1<<1
  }

  public class DlnaMaps
  {

    public static Dictionary<DlnaTypes, string> Mime = new Dictionary<DlnaTypes, string>();
    public static Dictionary<DlnaTypes, string> PN = new Dictionary<DlnaTypes, string>();

    public static Dictionary<string, DlnaTypes> Ext2Dlna = new Dictionary<string, DlnaTypes>();

    public static Dictionary<MediaTypes, List<string>> Media2Ext = new Dictionary<MediaTypes, List<string>>();
    public static Dictionary<string, MediaTypes> Ext2Media = new Dictionary<string, MediaTypes>();

    private static string[] avis = new string[] { "avi", "divx", "xvid" };
    private static string[] mkvs = new string[] { "mkv", "matroska", "mk3d", "webm" };
    private static string[] mpgs = new string[] { "mpg", "mpe", "mpeg", "mpg2", "mpeg2", "ts", "vob", "m2v" };
    private static string[] jpgs = new string[] { "jpg", "jpe", "jpeg", "jif", "jfif" };
    private static string[] avcs = new string[] { "avc", "mp4", "mov", "3gp", "3gpp", "flv" };

    static DlnaMaps()
    {
      PN.Add(DlnaTypes.MATROSKA, "DLNA.ORG_PN=MATROSKA");
      PN.Add(DlnaTypes.AVI, "DLNA.ORG_PN=AVI");
      PN.Add(DlnaTypes.MPEG, "DLNA.ORG_PN=MPEG1");
      PN.Add(DlnaTypes.JPEG, "DLNA.ORG_PN=JPEG");
      PN.Add(DlnaTypes.AVC, "DLNA.ORG_PN=AVC_TS_HD");

      Mime.Add(DlnaTypes.MATROSKA, "video/x-mkv");
      Mime.Add(DlnaTypes.AVI, "video/avi");
      Mime.Add(DlnaTypes.MPEG, "video/mpeg");
      Mime.Add(DlnaTypes.JPEG, "image/jpeg");
      Mime.Add(DlnaTypes.AVC, "video/mp4");

      foreach (var i in new[] { new { t = DlnaTypes.AVC, e = avcs }, new { t = DlnaTypes.AVI, e = avis }, new { t = DlnaTypes.JPEG, e = jpgs }, new { t = DlnaTypes.MATROSKA, e = mkvs }, new { t = DlnaTypes.MPEG, e = mpgs } }) {
        var t = i.t;
        foreach (var e in i.e) {
          Ext2Dlna.Add(e, t);
        }
      }

      InitMedia(new string[][] { avis, mkvs, mpgs, avcs }, MediaTypes.VIDEO);
      InitMedia(new string[][] { jpgs }, MediaTypes.IMAGE);
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
