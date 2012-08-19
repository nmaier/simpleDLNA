using System;
using System.IO;
using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.FileMediaServer
{
  internal class AudioFile : File, IMetaAudioItem
  {

    private string album;
    private string artist;
    private Cover cover = null;
    private string description;
    private TimeSpan? duration;
    private string genre;
    private bool initialized = false;
    private string performer;
    private string title;



    internal AudioFile(IMediaFolder aParent, FileInfo aFile, DlnaTypes aType)
      : base(aParent, aFile, aType, MediaTypes.AUDIO)
    {
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
        if (string.IsNullOrWhiteSpace(album)) {
          throw new NotSupportedException();
        }
        return album;
      }
    }

    public string MetaArtist
    {
      get
      {
        MaybeInit();
        if (string.IsNullOrWhiteSpace(artist)) {
          throw new NotSupportedException();
        }
        return artist;
      }
    }

    public string MetaDescription
    {
      get
      {
        MaybeInit();
        if (string.IsNullOrWhiteSpace(description)) {
          throw new NotSupportedException();
        }
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
        if (!string.IsNullOrWhiteSpace(title)) {
          return title;
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
        using (var tl = TagLib.File.Create(file.FullName)) {
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
                int width = 240, height = 240;
                var thumb = FileMediaServer.Cover.thumber.GetThumbnail(file.FullName, MediaTypes.IMAGE, pic.Data.ToStream(), ref width, ref height);
                if (thumb != null) {
                  cover = new Cover(thumb, width, height);
                }
              }
              catch (Exception ex) {
                Debug("Failed to generate thumb for " + file.FullName, ex);
              }
            }
          }
          catch (Exception ex) {
            Debug("Failed to transpose Tag props", ex);
          }
        }
      }
      catch (TagLib.CorruptFileException ex) {
        Debug("Failed to read metadata via taglib for file " + file.FullName, ex);
      }
      catch (Exception ex) {
        Warn("Unhandled exception reading metadata for file " + file.FullName, ex);
      }


      initialized = true;
    }
  }
}
