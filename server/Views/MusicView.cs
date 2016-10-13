using System.Globalization;
using System.Linq;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Views
{
  internal sealed class MusicView : BaseView
  {
    public override string Description => "Reorganizes files into a proper music collection";

    public override string Name => "music";

    private static void LinkTriple(TripleKeyedVirtualFolder folder,
      IMediaAudioResource r, string key1,
      string key2)
    {
      if (string.IsNullOrWhiteSpace(key1)) {
        return;
      }
      if (string.IsNullOrWhiteSpace(key2)) {
        return;
      }
      var targetFolder = folder
        .GetFolder(key1.StemCompareBase().First().ToString()
                     .ToUpper(CultureInfo.CurrentUICulture))
        .GetFolder(key1.StemNameBase());
      targetFolder
        .GetFolder(key2.StemNameBase())
        .AddResource(r);
      var allRes = new AlbumInTitleAudioResource(r);
      targetFolder
        .GetFolder("All Albums")
        .AddResource(allRes);
    }

    private static void SortFolder(VirtualFolder folder,
      TripleKeyedVirtualFolder artists,
      TripleKeyedVirtualFolder performers,
      DoubleKeyedVirtualFolder albums,
      SimpleKeyedVirtualFolder genres)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(f as VirtualFolder, artists, performers, albums, genres);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        var ai = i as IMediaAudioResource;
        if (ai == null) {
          continue;
        }
        var album = ai.MetaAlbum ?? "Unspecified album";
        albums.GetFolder(album.StemCompareBase().First().ToString().
                           ToUpper(CultureInfo.CurrentUICulture)).
          GetFolder(album.StemNameBase()).AddResource(i);
        LinkTriple(artists, ai, ai.MetaArtist, album);
        LinkTriple(performers, ai, ai.MetaPerformer, album);
        var genre = ai.MetaGenre;
        if (genre != null) {
          genres.GetFolder(genre.StemNameBase()).AddResource(i);
        }
      }
    }

    public override IMediaFolder Transform(IMediaFolder oldRoot)
    {
      var root = new VirtualClonedFolder(oldRoot);
      var artists = new TripleKeyedVirtualFolder(root, "Artists");
      var performers = new TripleKeyedVirtualFolder(root, "Performers");
      var albums = new DoubleKeyedVirtualFolder(root, "Albums");
      var genres = new SimpleKeyedVirtualFolder(root, "Genre");
      var folders = new VirtualFolder(root, "Folders");
      SortFolder(root, artists, performers, albums, genres);
      foreach (var f in root.ChildFolders.ToList()) {
        folders.AdoptFolder(f);
      }
      root.AdoptFolder(artists);
      root.AdoptFolder(performers);
      root.AdoptFolder(albums);
      root.AdoptFolder(genres);
      root.AdoptFolder(folders);
      return root;
    }

    private class AlbumInTitleAudioResource : AudioResourceDecorator
    {
      public AlbumInTitleAudioResource(IMediaAudioResource resource)
        : base(resource)
      {
      }

      public override string Title
      {
        get {
          var album = MetaAlbum;
          if (!string.IsNullOrWhiteSpace(album)) {
            return $"{album} — {base.Title}";
          }
          return base.Title;
        }
      }
    }

    private class TripleKeyedVirtualFolder
      : KeyedVirtualFolder<DoubleKeyedVirtualFolder>
    {
      public TripleKeyedVirtualFolder(IMediaFolder aParent, string aName)
        : base(aParent, aName)
      {
      }
    }
  }
}
