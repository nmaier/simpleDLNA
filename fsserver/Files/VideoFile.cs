using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.FileMediaServer
{
  [Serializable]
  internal sealed class VideoFile : BaseFile, IMediaVideoResource, ISerializable, IBookmarkable
  {
    private string[] actors;
    private long? bookmark;
    private string description;
    private string director;
    private TimeSpan? duration;
    private static readonly TimeSpan EmptyDuration =
      new TimeSpan(0);
    private string genre;
    private int? height;
    private bool initialized = false;
    private string title;
    private int? width;

    internal VideoFile(FileServer server, FileInfo aFile, DlnaMime aType)
      : base(server, aFile, aType, DlnaMediaTypes.Video)
    {
    }

    private VideoFile(SerializationInfo info, StreamingContext ctx)
      : this(info, ctx.Context as DeserializeInfo)
    {
    }

    private VideoFile(SerializationInfo info, DeserializeInfo di)
      : this(di.Server, di.Info, di.Type)
    {
      actors = info.GetValue("a", typeof(string[])) as string[];
      description = info.GetString("de");
      director = info.GetString("di");
      genre = info.GetString("g");
      title = info.GetString("t");
      try {
        width = info.GetInt32("w");
        height = info.GetInt32("h");
      }
      catch (Exception) {
        // no op
      }
      var ts = info.GetInt64("du");
      if (ts > 0) {
        duration = new TimeSpan(ts);
      }
      try {
        bookmark = info.GetInt64("b");
      }
      catch (Exception) {
        bookmark = 0;
      }
      initialized = true;
    }



    public long? Bookmark
    {
      get
      {
        return bookmark;
      }
      set
      {
        bookmark = value;
        Server.UpdateFileCache(this);
      }
    }

    public IEnumerable<string> MetaActors
    {
      get
      {
        MaybeInit();
        return actors;
      }
    }

    public string MetaDescription
    {
      get
      {
        MaybeInit();
        return description;
      }
    }

    public string MetaDirector
    {
      get
      {
        MaybeInit();
        return director;
      }
    }

    public TimeSpan? MetaDuration
    {
      get
      {
        MaybeInit();
        return duration;
      }
    }

    public string MetaGenre
    {
      get
      {
        MaybeInit();
        if (string.IsNullOrWhiteSpace(genre)) {
          throw new NotSupportedException();
        }
        return genre;
      }
    }

    public int? MetaHeight
    {
      get
      {
        MaybeInit();
        return height;
      }
    }

    public int? MetaWidth
    {
      get
      {
        MaybeInit();
        return width;
      }
    }

    public override IHeaders Properties
    {
      get
      {
        MaybeInit();
        var rv = base.Properties;
        if (description != null) {
          rv.Add("Description", description);
        }
        if (actors != null && actors.Length != 0) {
          rv.Add("Actors", string.Join(", ", actors));
        }
        if (director != null) {
          rv.Add("Director", director);
        }
        if (duration != null) {
          rv.Add("Duration", duration.Value.ToString("g"));
        }
        if (genre != null) {
          rv.Add("Genre", genre);
        }
        if (width != null && height != null) {
          rv.Add(
            "Resolution",
            string.Format("{0}x{1}", width.Value, height.Value)
            );
        }
        return rv;
      }
    }

    public override string Title
    {
      get
      {
        if (!string.IsNullOrWhiteSpace(title)) {
          return string.Format("{0} — {1}", base.Title, title);
        }
        return base.Title;
      }
    }

    public void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      if (info == null) {
        throw new ArgumentNullException("info");
      }
      MaybeInit();
      info.AddValue("a", actors, typeof(string[]));
      info.AddValue("de", description);
      info.AddValue("di", director);
      info.AddValue("g", genre);
      info.AddValue("t", title);
      info.AddValue("w", width);
      info.AddValue("h", height);
      info.AddValue("b", bookmark);
      info.AddValue(
        "du",
        duration.GetValueOrDefault(EmptyDuration).Ticks
        );
    }

    private void MaybeInit()
    {
      if (initialized) {
        return;
      }

      try {
        using (var tl = TagLib.File.Create(new TagLibFileAbstraction(Item))) {
          try {
            duration = tl.Properties.Duration;
            if (duration.HasValue && duration.Value.TotalSeconds < 0.1) {
              duration = null;
            }
            width = tl.Properties.VideoWidth;
            height = tl.Properties.VideoHeight;
          }
          catch (Exception ex) {
            Debug("Failed to transpose Properties props", ex);
          }

          try {
            var t = tl.Tag;
            genre = t.FirstGenre;
            title = t.Title;
            description = t.Comment;
            director = t.FirstComposerSort;
            if (string.IsNullOrWhiteSpace(director)) {
              director = t.FirstComposer;
            }
            actors = t.PerformersSort;
            if (actors == null || actors.Length == 0) {
              actors = t.PerformersSort;
              if (actors == null || actors.Length == 0) {
                actors = t.Performers;
                if (actors == null || actors.Length == 0) {
                  actors = t.AlbumArtists;
                }
              }
            }
          }
          catch (Exception ex) {
            Debug("Failed to transpose Tag props", ex);
          }
        }

        initialized = true;

        Server.UpdateFileCache(this);
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (TagLib.UnsupportedFormatException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + Item.FullName, ex);
      }
    }
  }
}
