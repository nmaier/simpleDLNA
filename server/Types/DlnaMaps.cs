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

    private static readonly string[] ext3GPP =
    {"3gp", "3gpp"};

    private static readonly string[] extAAC =
    {"aac", "mp4a", "m4a"};

    private static readonly string[] extAVC =
    {"avc", "mp4", "m4v", "mov"};

    private static readonly string[] extAVI =
    {"avi", "divx", "xvid"};

    private static readonly string[] extFLV =
    {"flv"};

    private static readonly string[] extGIF =
    {"gif"};

    private static readonly string[] extJPEG =
    {"jpg", "jpe", "jpeg", "jif", "jfif"};

    private static readonly string[] extMKV =
    {"mkv", "matroska", "mk3d", "webm"};

    private static readonly string[] extMP3 =
    {"mp3", "mp3p", "mp3x", "mp3a", "mpa"};

    private static readonly string[] extMP2 =
    {"mp2"};

    private static readonly string[] extMPEG =
    {"mpg", "mpe", "mpeg", "mpg2", "mpeg2", "ts", "vob", "m2v"};

    private static readonly string[] extOGV =
    {"ogm", "ogv"};

    private static readonly string[] extPNG =
    {"png"};

    private static readonly string[] extRAWAUDIO =
    {"wav"};

    private static readonly string[] extVORBIS =
    {"ogg", "oga"};

    private static readonly string[] extWMV =
    {"wmv", "asf", "wma", "wmf"};

    private static readonly string[] extFLAC =
    {"flac"};

    public static readonly Dictionary<DlnaMime, List<string>> Dlna2Ext =
      new Dictionary<DlnaMime, List<string>>();

    public static readonly Dictionary<string, DlnaMime> Ext2Dlna =
      new Dictionary<string, DlnaMime>();

    public static readonly Dictionary<string, DlnaMediaTypes> Ext2Media =
      new Dictionary<string, DlnaMediaTypes>();

    public static readonly Dictionary<DlnaMediaTypes, List<string>> Media2Ext =
      new Dictionary<DlnaMediaTypes, List<string>>();

    public static readonly Dictionary<DlnaMime, string> Mime = new Dictionary<DlnaMime, string>
    {
      {DlnaMime.AudioAAC, "audio/aac"},
      {DlnaMime.AudioFLAC, "audio/flac"},
      {DlnaMime.AudioMP2, "audio/mpeg"},
      {DlnaMime.AudioMP3, "audio/mpeg"},
      {DlnaMime.AudioRAW, "audio/L16;rate=44100;channels=2"},
      {DlnaMime.AudioVORBIS, "audio/ogg"},
      {DlnaMime.ImageGIF, "image/gif"},
      {DlnaMime.ImageJPEG, "image/jpeg"},
      {DlnaMime.ImagePNG, "image/png"},
      {DlnaMime.SubtitleSRT, "smi/caption"},
      {DlnaMime.Video3GPP, "video/3gpp"},
      {DlnaMime.VideoAVC, "video/mp4"},
      {DlnaMime.VideoAVI, "video/avi"},
      {DlnaMime.VideoFLV, "video/flv"},
      {DlnaMime.VideoMKV, "video/x-mkv"},
      {DlnaMime.VideoMPEG, "video/mpeg"},
      {DlnaMime.VideoOGV, "video/ogg"},
      {DlnaMime.VideoWMV, "video/x-ms-wmv"}
    };

    public static readonly Dictionary<DlnaMime, List<string>> AllPN = new Dictionary<DlnaMime, List<string>>
    {
      {
        DlnaMime.AudioAAC, new List<string>
        {
          "AAC"
        }
      },
      {
        DlnaMime.AudioFLAC, new List<string>
        {
          "FLAC"
        }
      },
      {
        DlnaMime.AudioMP2, new List<string>
        {
          "MP2_MPS"
        }
      },
      {
        DlnaMime.AudioMP3, new List<string>
        {
          "MP3"
        }
      },
      {
        DlnaMime.AudioRAW, new List<string>
        {
          "LPCM"
        }
      },
      {
        DlnaMime.AudioVORBIS, new List<string>
        {
          "OGG"
        }
      },
      {
        DlnaMime.ImageGIF, new List<string>
        {
          "GIF",
          "GIF_LRG",
          "GIF_MED",
          "GIF_SM"
        }
      },
      {
        DlnaMime.ImageJPEG, new List<string>
        {
          "JPEG",
          "JPEG_LRG",
          "JPEG_MED",
          "JPEG_SM",
          "JPEG_TN"
        }
      },
      {
        DlnaMime.ImagePNG, new List<string>
        {
          "PNG",
          "PNG_LRG",
          "PNG_MED",
          "PNG_SM",
          "PNG_TN"
        }
      },
      {
        DlnaMime.SubtitleSRT, new List<string>
        {
          "SRT"
        }
      },
      {
        DlnaMime.Video3GPP, new List<string>
        {
          "MPEG4_P2_3GPP_SP_L0B_AMR",
          "AVC_3GPP_BL_QCIF15_AAC",
          "MPEG4_H263_3GPP_P0_L10_AMR",
          "MPEG4_H263_MP4_P0_L10_AAC",
          "MPEG4_P2_3GPP_SP_L0B_AAC"
        }
      },
      {
        DlnaMime.VideoAVC, new List<string>
        {
          "AVC_MP4_MP_SD_AAC_MULT5",
          "AVC_MP4_HP_HD_AAC",
          "AVC_MP4_HP_HD_DTS",
          "AVC_MP4_LPCM",
          "AVC_MP4_MP_SD_AC3",
          "AVC_MP4_MP_SD_DTS",
          "AVC_MP4_MP_SD_MPEG1_L3",
          "AVC_TS_HD_50_LPCM_T",
          "AVC_TS_HD_DTS_ISO",
          "AVC_TS_HD_DTS_T",
          "AVC_TS_HP_HD_MPEG1_L2_ISO",
          "AVC_TS_HP_HD_MPEG1_L2_T",
          "AVC_TS_HP_SD_MPEG1_L2_ISO",
          "AVC_TS_HP_SD_MPEG1_L2_T",
          "AVC_TS_MP_HD_AAC_MULT5",
          "AVC_TS_MP_HD_AAC_MULT5_ISO",
          "AVC_TS_MP_HD_AAC_MULT5_T",
          "AVC_TS_MP_HD_AC3",
          "AVC_TS_MP_HD_AC3_ISO",
          "AVC_TS_MP_HD_AC3_T",
          "AVC_TS_MP_HD_MPEG1_L3",
          "AVC_TS_MP_HD_MPEG1_L3_ISO",
          "AVC_TS_MP_HD_MPEG1_L3_T",
          "AVC_TS_MP_SD_AAC_MULT5",
          "AVC_TS_MP_SD_AAC_MULT5_ISO",
          "AVC_TS_MP_SD_AAC_MULT5_T",
          "AVC_TS_MP_SD_AC3",
          "AVC_TS_MP_SD_AC3_ISO",
          "AVC_TS_MP_SD_AC3_T",
          "AVC_TS_MP_SD_MPEG1_L3",
          "AVC_TS_MP_SD_MPEG1_L3_ISO",
          "AVC_TS_MP_SD_MPEG1_L3_T"
        }
      },
      {
        DlnaMime.VideoAVI, new List<string>
        {
          "AVI"
        }
      },
      {
        DlnaMime.VideoFLV, new List<string>
        {
          "FLV"
        }
      },
      {
        DlnaMime.VideoMKV, new List<string>
        {
          "MATROSKA"
        }
      },
      {
        DlnaMime.VideoMPEG, new List<string>
        {
          "MPEG1",
          "MPEG_PS_PAL",
          "MPEG_PS_NTSC",
          "MPEG_TS_SD_EU",
          "MPEG_TS_SD_EU_T",
          "MPEG_TS_SD_EU_ISO",
          "MPEG_TS_SD_NA",
          "MPEG_TS_SD_NA_T",
          "MPEG_TS_SD_NA_ISO",
          "MPEG_TS_SD_KO",
          "MPEG_TS_SD_KO_T",
          "MPEG_TS_SD_KO_ISO",
          "MPEG_TS_JP_T"
        }
      },
      {
        DlnaMime.VideoOGV, new List<string>
        {
          "OGV"
        }
      },
      {
        DlnaMime.VideoWMV, new List<string>
        {
          "WMV_FULL",
          "WMV_BASE",
          "WMVHIGH_FULL",
          "WMVHIGH_BASE",
          "WMVHIGH_PRO",
          "WMVMED_FULL",
          "WMVMED_BASE",
          "WMVMED_PRO",
          "VC1_ASF_AP_L1_WMA",
          "VC1_ASF_AP_L2_WMA",
          "VC1_ASF_AP_L3_WMA"
        }
      }
    };

    public static readonly Dictionary<DlnaMime, string> MainPN = GenerateMainPN();

    public static readonly string ProtocolInfo = GenerateProtocolInfo();

    static DlnaMaps()
    {
      var extToDLNA = new[]
      {
        new
        {t = DlnaMime.AudioAAC, e = extAAC},
        new
        {t = DlnaMime.AudioFLAC, e = extFLAC},
        new
        {t = DlnaMime.AudioMP2, e = extMP2},
        new
        {t = DlnaMime.AudioMP3, e = extMP3},
        new
        {t = DlnaMime.AudioRAW, e = extRAWAUDIO},
        new
        {t = DlnaMime.AudioVORBIS, e = extVORBIS},
        new
        {t = DlnaMime.ImageGIF, e = extGIF},
        new
        {t = DlnaMime.ImageJPEG, e = extJPEG},
        new
        {t = DlnaMime.ImagePNG, e = extPNG},
        new
        {t = DlnaMime.Video3GPP, e = ext3GPP},
        new
        {t = DlnaMime.VideoAVC, e = extAVC},
        new
        {t = DlnaMime.VideoAVI, e = extAVI},
        new
        {t = DlnaMime.VideoFLV, e = extFLV},
        new
        {t = DlnaMime.VideoMKV, e = extMKV},
        new
        {t = DlnaMime.VideoMPEG, e = extMPEG},
        new
        {t = DlnaMime.VideoOGV, e = extOGV},
        new
        {t = DlnaMime.VideoWMV, e = extWMV}
      };

      foreach (var i in extToDLNA) {
        var t = i.t;
        foreach (var e in i.e) {
          Ext2Dlna.Add(e.ToUpperInvariant(), t);
        }
        Dlna2Ext.Add(i.t, new List<string>(i.e));
      }

      InitMedia(
        new[] {ext3GPP, extAVI, extAVC, extFLV, extMKV, extMPEG, extOGV, extWMV},
        DlnaMediaTypes.Video);
      InitMedia(
        new[] {extJPEG, extPNG, extGIF},
        DlnaMediaTypes.Image);
      InitMedia(
        new[] {extAAC, extFLAC, extMP2, extMP3, extRAWAUDIO, extVORBIS},
        DlnaMediaTypes.Audio);
    }

    private static string GenerateProtocolInfo()
    {
      var pns = (from p in AllPN
                 let mime = Mime[p.Key]
                 from pn in p.Value
                 select
                   string.Format("http-get:*:{1}:DLNA.ORG_PN={0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={2}", pn,
                                 mime, DefaultStreaming)).ToList();
      return string.Join(",", pns);
    }

    private static void InitMedia(string[][] k, DlnaMediaTypes t)
    {
      foreach (var i in k) {
        var e = (from ext in i
                 select ext.ToUpperInvariant()).ToList();
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
      return $"{(ulong)flags:X8}{0:D24}";
    }

    public static Dictionary<DlnaMime, string> GenerateMainPN()
    {
      return AllPN.ToDictionary(p => p.Key, p => p.Value.FirstOrDefault());
    }
  }
}
