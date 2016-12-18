using System;
using System.Globalization;
using System.IO;
using System.Text;
using log4net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public sealed class Subtitle : IMediaResource
  {
    [NonSerialized] private static readonly ILog logger =
      LogManager.GetLogger(typeof (Subtitle));

    [NonSerialized] private static readonly string[] exts =
    {
      ".srt", ".SRT",
      ".ass", ".ASS",
      ".ssa", ".SSA",
      ".sub", ".SUB",
      ".vtt", ".VTT"
    };

    [NonSerialized] private byte[] encodedText;

    private string text;

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

    public bool HasSubtitle => !string.IsNullOrWhiteSpace(text);

    public DateTime InfoDate => DateTime.UtcNow;

    public long? InfoSize
    {
      get {
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

    public IMediaCoverResource Cover
    {
      get { throw new NotImplementedException(); }
    }

    public string Id
    {
      get { return Path; }
      set { throw new NotImplementedException(); }
    }

    public DlnaMediaTypes MediaType
    {
      get { throw new NotImplementedException(); }
    }

    public string Path => "ad-hoc-subtitle:";

    public string PN => DlnaMaps.MainPN[Type];

    public IHeaders Properties
    {
      get {
        var rv = new RawHeaders {{"Type", Type.ToString()}};
        if (InfoSize.HasValue) {
          rv.Add("SizeRaw", InfoSize.ToString());
          rv.Add("Size", InfoSize.Value.FormatFileSize());
        }
        rv.Add("Date", InfoDate.ToString(CultureInfo.InvariantCulture));
        rv.Add("DateO", InfoDate.ToString("o"));
        return rv;
      }
    }

    public string Title
    {
      get { throw new NotImplementedException(); }
    }

    public DlnaMime Type => DlnaMime.SubtitleSRT;

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
            logger.DebugFormat("Loaded subtitle from {0}", sti.FullName);
          }
          catch (NotSupportedException) {
          }
          catch (Exception ex) {
            logger.Debug($"Failed to get subtitle from {sti.FullName}", ex);
          }
        }
        try {
          text = FFmpeg.GetSubtitleSubrip(file);
          logger.DebugFormat("Loaded subtitle from {0}", file.FullName);
        }
        catch (NotSupportedException ex) {
          logger.Debug($"Subtitle not supported {file.FullName}", ex);
        }
        catch (Exception ex) {
          logger.Debug($"Failed to get subtitle from {file.FullName}", ex);
        }
      }
      catch (Exception ex) {
        logger.Error($"Failed to load subtitle for {file.FullName}", ex);
      }
    }
  }
}
