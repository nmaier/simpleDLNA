using NMaier.SimpleDlna.Server;
using System;
using System.IO;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.FileMediaServer
{
  [Serializable]
  internal sealed class AudioFile
    : BaseFile, IMediaAudioResource, ISerializable
  {
    private string album;

    private string artist;

    private string description;

    private TimeSpan? duration;

    private static readonly TimeSpan EmptyDuration = new TimeSpan(0);

    private string genre;

    private bool initialized = false;

    private string performer;

    private string title;

    private int? track;

    private AudioFile(SerializationInfo info, DeserializeInfo di)
      : this(di.Server, di.Info, di.Type)
    {
      album = info.GetString("al");
      artist = info.GetString("ar");
      genre = info.GetString("g");
      performer = info.GetString("p");
      title = info.GetString("ti");
      try {
        track = info.GetInt32("tr");
      }
      catch (Exception) {
        // no op
      }
      var ts = info.GetInt64("d");
      if (ts > 0) {
        duration = new TimeSpan(ts);
      }
      initialized = true;
    }

    private AudioFile(SerializationInfo info, StreamingContext ctx)
      :
      this(info, ctx.Context as DeserializeInfo)
    {
    }

    internal AudioFile(FileServer server, FileInfo aFile, DlnaMime aType)
      : base(server, aFile, aType, DlnaMediaTypes.Audio)
    {
    }

    public override IMediaCoverResource Cover
    {
      get
      {
        if (cover == null && !LoadCoverFromCache()) {
          MaybeInit();
        }
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

    public int? MetaTrack
    {
      get
      {
        MaybeInit();
        return track;
      }
    }

    public override IHeaders Properties
    {
      get
      {
        MaybeInit();
        var rv = base.Properties;
        if (album != null) {
          rv.Add("Album", album);
        }
        if (artist != null) {
          rv.Add("Artist", artist);
        }
        if (description != null) {
          rv.Add("Description", description);
        }
        if (duration != null) {
          rv.Add("Duration", duration.Value.ToString("g"));
        }
        if (genre != null) {
          rv.Add("Genre", genre);
        }
        if (performer != null) {
          rv.Add("Performer", performer);
        }
        if (track != null) {
          rv.Add("Track", track.Value.ToString());
        }
        return rv;
      }
    }

    public override string Title
    {
      get
      {
        MaybeInit();
        if (!string.IsNullOrWhiteSpace(title)) {
          if (track.HasValue) {
            return string.Format(
              "{0:D2}. — {1}",
              track.Value,
              title
              );
          }
          return title;
        }
        return base.Title;
      }
    }

    private void InitCover(TagLib.Tag tag)
    {
      TagLib.IPicture pic = null;
      foreach (var p in tag.Pictures) {
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
          Logger.Debug("Failed to generate thumb for " + Item.FullName, ex);
        }
      }
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
          }
          catch (Exception ex) {
            Logger.Debug("Failed to transpose Properties props", ex);
          }

          try {
            var t = tl.Tag;
            SetProperties(t);
            InitCover(t);
          }
          catch (Exception ex) {
            Logger.Debug("Failed to transpose Tag props", ex);
          }
        }

        initialized = true;

        Server.UpdateFileCache(this);
      }
      catch (TagLib.CorruptFileException ex) {
        Logger.Debug(
          "Failed to read meta data via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (TagLib.UnsupportedFormatException ex) {
        Logger.Debug(
          "Failed to read meta data via taglib for file " + Item.FullName, ex);
        initialized = true;
      }
      catch (Exception ex) {
        Logger.Warn(
          "Unhandled exception reading meta data for file " + Item.FullName,
          ex);
      }
    }

    private void SetProperties(TagLib.Tag tag)
    {
      genre = tag.FirstGenre;
      if (string.IsNullOrWhiteSpace(genre)) {
        genre = null;
      }

      if (tag.Track != 0 && tag.Track < (1 << 10)) {
        track = (int)tag.Track;
      }


      title = tag.Title;
      if (string.IsNullOrWhiteSpace(title)) {
        title = null;
      }

      description = tag.Comment;
      if (string.IsNullOrWhiteSpace(description)) {
        description = null;
      }

      if (string.IsNullOrWhiteSpace(artist)) {
        performer = tag.JoinedPerformers;
      }
      else {
        performer = tag.JoinedPerformersSort;
      }
      if (string.IsNullOrWhiteSpace(performer)) {
        performer = null;
      }

      artist = tag.JoinedAlbumArtists;
      if (string.IsNullOrWhiteSpace(artist)) {
        artist = tag.JoinedComposers;
      }
      if (string.IsNullOrWhiteSpace(artist)) {
        artist = null;
      }

      album = tag.AlbumSort;
      if (string.IsNullOrWhiteSpace(album)) {
        album = tag.Album;
      }
      if (string.IsNullOrWhiteSpace(album)) {
        album = null;
      }
    }

    public override int CompareTo(IMediaItem other)
    {
      if (track.HasValue) {
        var oa = other as AudioFile;
        int rv;
        if (oa != null && oa.track.HasValue &&
          (rv = track.Value.CompareTo(oa.track.Value)) != 0) {
          return rv;
        }
      }
      return base.CompareTo(other);
    }

    public void GetObjectData(SerializationInfo info, StreamingContext ctx)
    {
      if (info == null) {
        throw new ArgumentNullException("info");
      }
      info.AddValue("al", album);
      info.AddValue("ar", artist);
      info.AddValue("g", genre);
      info.AddValue("p", performer);
      info.AddValue("ti", title);
      info.AddValue("tr", track);
      info.AddValue(
        "d", duration.GetValueOrDefault(EmptyDuration).Ticks);
    }

    public override void LoadCover()
    {
      // No op
    }
  }
}
