using System;
using System.Collections.Generic;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer
{
  internal class VideoFile : File, IMetaVideoItem
  {

    private string[] actors;
    private string description;
    private string director;
    private TimeSpan? duration;
    private string genre;
    private uint? height;
    private bool initialized = false;
    private string title;
    private uint? width;



    internal VideoFile(IFileServerFolder aParent, FileInfo aFile, DlnaTypes aType)
      : base(aParent, aFile, aType, MediaTypes.VIDEO)
    {
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
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + Item.FullName, ex);
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + Item.FullName, ex);
      }


      initialized = true;
    }
  }
}
