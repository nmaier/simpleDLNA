using System;
using System.IO;
using System.Runtime.Serialization;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer.Files
{
  [Serializable]
  internal class AudioFile : BaseFile, IMetaAudioItem, ISerializable
  {
    private static readonly TimeSpan EmptyDuration = new TimeSpan(0);

    private string album;
    private string artist;
    private Cover cover = null;
    private string description;
    private TimeSpan? duration;
    private string genre;
    private bool initialized = false;
    private string performer;
    private string title;



    internal AudioFile(BaseFolder aParent, FileInfo aFile, DlnaTypes aType)
      : base(aParent, aFile, aType, MediaTypes.AUDIO)
    {
    }

    protected AudioFile(SerializationInfo info, StreamingContext ctx) :
      this(null, (ctx.Context as DeserializeInfo).Info, (ctx.Context as DeserializeInfo).Type)
    {
      album = info.GetString("al");
      artist = info.GetString("ar");
      genre = info.GetString("g");
      performer = info.GetString("p");
      title = info.GetString("ti");
      var ts = info.GetInt64("d");
      if (ts > 0) {
        duration = new TimeSpan(ts);
      }
      try {
        cover = info.GetValue("c", typeof(Cover)) as Cover;
      }
      catch (SerializationException) { }

      initialized = true;
    }



    public override IMediaCoverResource Cover
    {
      get
      {
        MaybeInit();
        return cover;
      }
    }

    public string MetaAlbum
    {
      get
      {
        MaybeInit();
        return album;
      }
    }

    public string MetaArtist
    {
      get
      {
        MaybeInit();
        return artist;
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
        return genre;
      }
    }

    public string MetaPerformer
    {
      get
      {
        MaybeInit();
        return performer;
      }
    }

    public override string Title
    {
      get
      {
        MaybeInit();
        if (!string.IsNullOrWhiteSpace(title)) {
          return title;
        }
        return base.Title;
      }
    }




    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      info.AddValue("ty", (Int32)Type);
      info.AddValue("al", album);
      info.AddValue("ar", artist);
      info.AddValue("g", genre);
      info.AddValue("p", performer);
      info.AddValue("ti", title);
      info.AddValue("d", duration.GetValueOrDefault(EmptyDuration).Ticks);
      if (cover != null) {
        info.AddValue("c", cover, typeof(Cover));
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
          }
          catch (Exception ex) {
            Debug("Failed to transpose Properties props", ex);
          }

          try {
            var t = tl.Tag;
            genre = t.FirstGenre;
            if (string.IsNullOrWhiteSpace(genre)) {
              genre = null;
            }

            title = t.Title;
            if (string.IsNullOrWhiteSpace(title)) {
              title = null;
            }

            description = t.Comment;
            if (string.IsNullOrWhiteSpace(description)) {
              description = null;
            }

            performer = t.JoinedPerformersSort;
            if (string.IsNullOrWhiteSpace(artist)) {
              performer = t.JoinedPerformers;
            }
            if (string.IsNullOrWhiteSpace(performer)) {
              performer = null;
            }

            artist = t.JoinedAlbumArtists;
            if (string.IsNullOrWhiteSpace(artist)) {
              artist = t.JoinedComposers;
            }
            if (string.IsNullOrWhiteSpace(artist)) {
              artist = null;
            }

            album = t.AlbumSort;
            if (string.IsNullOrWhiteSpace(album)) {
              album = t.Album;
            }
            if (string.IsNullOrWhiteSpace(album)) {
              album = null;
            }

            TagLib.IPicture pic = null;
            foreach (var p in t.Pictures) {
              if (p.Type == TagLib.PictureType.FrontCover) {
                pic = p;
                break;
              }
              switch (p.Type) {
                case TagLib.PictureType.Other:
                case TagLib.PictureType.OtherFileIcon:
                case TagLib.PictureType.FileIcon:
                  pic = p;
                  break;
                default:
                  if (pic == null) {
                    pic = p;
                  }
                  break;
              }
            }
            if (pic != null) {
              try {
                cover = new Cover(Item, pic.Data.ToStream());
              }
              catch (Exception ex) {
                Debug("Failed to generate thumb for " + Item.FullName, ex);
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

      Parent.Server.UpdateFileCache(this);


      initialized = true;
    }
  }
}
