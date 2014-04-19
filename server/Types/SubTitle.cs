using System;
using System.IO;
using System.Text;
using log4net;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public class SubTitle : IMediaResource
  {
    [NonSerialized]
    static private readonly ILog logger = LogManager.GetLogger(typeof(SubTitle));
    [NonSerialized]
    static private readonly string[] exts = new string[] {
      ".srt", ".SRT",
      ".ass", ".ASS",
      ".ssa", ".SSA",
      ".sub", ".SUB",
      ".vtt", ".VTT"
    };
    
    private string text = null;

    [NonSerialized]
    public byte[] encodedText = null;

    public SubTitle(FileInfo file)
    {
      Load(file);
    }

    public SubTitle(string text)
    {
      this.text = text;
    }

    public SubTitle() { }

    private void Load(FileInfo file)
    {
      try {
        // Try external
        foreach (var i in exts) {
          var sti = new FileInfo(System.IO.Path.ChangeExtension(file.FullName, i));
          try {
            if (!sti.Exists) {
              sti = new FileInfo(file.FullName + i);
            }
            if (!sti.Exists) {
              continue;
            }
            text = FFmpeg.GetSubtitleSRT(sti);
          }
          catch (NotSupportedException) {
          }
          catch (Exception ex) {
            logger.Debug(string.Format("Failed to get subtitle from {0}", sti.FullName), ex);
          }
        }
        try {
          text = FFmpeg.GetSubtitleSRT(file);
        }
        catch (NotSupportedException) {
        }
        catch (Exception ex) {
          logger.Debug(string.Format("Failed to get subtitle from {0}", file.FullName), ex);
        }
      }
      catch (Exception ex) {
        logger.Error(string.Format("Failed to load subtitle for {0}", file.FullName), ex);
      }
    }

    public bool HasSubtitle
    {
      get {
        return !string.IsNullOrWhiteSpace(text);
      }
    }


    public Stream Content
    {
      get
      {
        if (!HasSubtitle) {
          throw new NotSupportedException();
        }
        if (encodedText == null) {
          encodedText = Encoding.UTF8.GetBytes(text);
        }
        return new MemoryStream(encodedText, false);
      }
    }
    public IMediaCoverResource Cover
    {
      get
      {
        throw new NotImplementedException();
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
          using (var s = Content) {
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
        return DlnaMaps.PN[Type];
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
        return DlnaMime.SRT;
      }
    }


    public int CompareTo(IMediaItem other)
    {
      throw new NotImplementedException();
    }
  }
}
