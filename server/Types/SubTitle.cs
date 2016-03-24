using log4net;
using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Text;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public sealed class Subtitle : IMediaResource
  {
    [NonSerialized]
    private byte[] encodedText = null;

    [NonSerialized]
    private static readonly ILog logger =
      LogManager.GetLogger(typeof(Subtitle));

    [NonSerialized]
    private static readonly string[] exts = new string[] {
      ".srt", ".SRT",
      ".ass", ".ASS",
      ".ssa", ".SSA",
      ".sub", ".SUB",
      ".vtt", ".VTT"
      };

    private string text = null;

    public Subtitle()
    {
    }

    public Subtitle(FileInfo file)
    {
      Load(file);
    }

    public Subtitle(string text)
    {
      this.text = text;
    }

    public IMediaCoverResource Cover
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public bool HasSubtitle
    {
      get
      {
        return !string.IsNullOrWhiteSpace(text);
      }
    }

    public string Id
    {
      get
      {
        return Path;
      }
      set
      {
        throw new NotImplementedException();
      }
    }

    public DateTime InfoDate
    {
      get
      {
        return DateTime.UtcNow;
      }
    }

    public long? InfoSize
    {
      get
      {
        try {
          using (var s = CreateContentStream()) {
            return s.Length;
          }
        }
        catch (Exception) {
          return null;
        }
      }
    }

    public DlnaMediaTypes MediaType
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public string Path
    {
      get
      {
        return "ad-hoc-subtitle:";
      }
    }

    public string PN
    {
      get
      {
        return DlnaMaps.MainPN[Type];
      }
    }

    public IHeaders Properties
    {
      get
      {
        var rv = new RawHeaders();
        rv.Add("Type", Type.ToString());
        if (InfoSize.HasValue) {
          rv.Add("SizeRaw", InfoSize.ToString());
          rv.Add("Size", InfoSize.Value.FormatFileSize());
        }
        rv.Add("Date", InfoDate.ToString());
        rv.Add("DateO", InfoDate.ToString("o"));
        return rv;
      }
    }

    public string Title
    {
      get
      {
        throw new NotImplementedException();
      }
    }

    public DlnaMime Type
    {
      get
      {
        return DlnaMime.SubtitleSRT;
      }
    }

    private void Load(FileInfo file)
    {
      try {
        // Try external
        foreach (var i in exts) {
          var sti = new FileInfo(
            System.IO.Path.ChangeExtension(file.FullName, i));
          try {
            if (!sti.Exists) {
              sti = new FileInfo(file.FullName + i);
            }
            if (!sti.Exists) {
              continue;
            }
            text = FFmpeg.GetSubtitleSubrip(sti);
            logger.DebugFormat("Loaded subtitle from {9}", sti.FullName);
          }
          catch (NotSupportedException) {
          }
          catch (Exception ex) {
            logger.Debug(string.Format(
              "Failed to get subtitle from {0}", sti.FullName), ex);
          }
        }
        try {
          text = FFmpeg.GetSubtitleSubrip(file);
          logger.DebugFormat("Loaded subtitle from {0}", file.FullName);
        }
        catch (NotSupportedException ex) {
          logger.Debug(string.Format("Subtitle not supported {0}", file.FullName), ex);
        }
        catch (Exception ex) {
          logger.Debug(string.Format(
            "Failed to get subtitle from {0}", file.FullName), ex);
        }
      }
      catch (Exception ex) {
        logger.Error(string.Format(
          "Failed to load subtitle for {0}", file.FullName), ex);
      }
    }

    public int CompareTo(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public Stream CreateContentStream()
    {
      if (!HasSubtitle) {
        throw new NotSupportedException();
      }
      if (encodedText == null) {
        encodedText = Encoding.UTF8.GetBytes(text);
      }
      return new MemoryStream(encodedText, false);
    }

    public bool Equals(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public string ToComparableTitle()
    {
      throw new NotImplementedException();
    }
  }
}
