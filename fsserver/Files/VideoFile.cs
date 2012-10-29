using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using NMaier.SimpleDlna.FileMediaServer.Folders;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer.Files
{
  [Serializable]
  internal sealed class VideoFile : BaseFile, IMetaVideoItem, ISerializable, IBookmarkable
  {

    private string[] actors;
    private ulong? bookmark;
    private string description;
    private string director;
    private TimeSpan? duration;
    private static readonly TimeSpan EmptyDuration = new TimeSpan(0);
    private string genre;
    private uint? height;
    private bool initialized = false;
    private string title;
    private uint? width;



    internal VideoFile(BaseFolder aParent, FileInfo aFile, DlnaType aType)
      : base(aParent, aFile, aType, MediaTypes.VIDEO)
    {
    }

    private VideoFile(SerializationInfo info, StreamingContext ctx)
      : this(null, (ctx.Context as DeserializeInfo).Info, (ctx.Context as DeserializeInfo).Type)
    {
      actors = info.GetValue("a", typeof(string[])) as string[];
      description = info.GetString("de");
      director = info.GetString("di");
      genre = info.GetString("g");
      title = info.GetString("t");
      width = info.GetUInt32("w");
      height = info.GetUInt32("h");
      var ts = info.GetInt64("du");
      if (ts > 0) {
        duration = new TimeSpan(ts);
      }
      try {
        bookmark = info.GetUInt64("b");
      }
      catch (Exception) { }
      initialized = true;
    }



    public ulong? Bookmark
    {
      get { return bookmark; }
      set
      {
        bookmark = value;
        Parent.Server.UpdateFileCache(this);
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

    public uint? MetaHeight
    {
      get
      {
        MaybeInit();
        return height;
      }
    }

    public uint? MetaWidth
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
          rv.Add("Resolution", string.Format("{0}x{1}", width.Value, height.Value));
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




    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      MaybeInit();
      info.AddValue("a", actors, typeof(string[]));
      info.AddValue("de", description);
      info.AddValue("di", director);
      info.AddValue("g", genre);
      info.AddValue("t", title);
      info.AddValue("w", width);
      info.AddValue("h", height);
      info.AddValue("b", bookmark);
      info.AddValue("du", duration.GetValueOrDefault(EmptyDuration).Ticks);
    }

    private void MaybeInit()
    {
      if (initialized) {
        return;
      }

      try {
        using (var tl = TagLib.File.Create(Item.FullName)) {
          try {
            duration = tl.Properties.Duration;
            if (duration.HasValue && duration.Value.TotalSeconds < 0.1) {
              duration = null;
            }
            width = (uint)tl.Properties.VideoWidth;
            height = (uint)tl.Properties.VideoHeight;
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

        Parent.Server.UpdateFileCache(this);
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + Item.FullName, ex);
      }
    }
  }
}
