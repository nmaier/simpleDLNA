using System.Linq;
using NMaier.sdlna.FileMediaServer.Files;
using NMaier.sdlna.FileMediaServer.Folders;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer.Views
{
  class MusicView : IView
  {
    private class SimpleKeyedVirtualFolder : KeyedVirtualFolder<VirtualFolder>
    {

      public SimpleKeyedVirtualFolder(FileServer server, IFileServerFolder aParent, string aName)
        : base(server, aParent, aName)
      {
      }

      public SimpleKeyedVirtualFolder() { }
    }

    private class DoubleKeyedVirtualFolder : KeyedVirtualFolder<SimpleKeyedVirtualFolder>
    {

      public DoubleKeyedVirtualFolder(FileServer server, IFileServerFolder aParent, string aName)
        : base(server, aParent, aName)
      {
      }

      public DoubleKeyedVirtualFolder() { }
    }

    private class TripleKeyedVirtualFolder : KeyedVirtualFolder<DoubleKeyedVirtualFolder>
    {

      public TripleKeyedVirtualFolder(FileServer server, IFileServerFolder aParent, string aName)
        : base(server, aParent, aName)
      {
      }

      public TripleKeyedVirtualFolder() { }
    }


    public string Description
    {
      get { return "Reorganizes files into a proper music collection"; }
    }

    public string Name
    {
      get { return "music"; }
    }




    public void Transform(FileServer Server, IMediaFolder Root)
    {
      var root = Root as IFileServerFolder;
      var artists = new TripleKeyedVirtualFolder(Server, root, "Artists");
      var performers = new TripleKeyedVirtualFolder(Server, root, "Performers");
      var albums = new DoubleKeyedVirtualFolder(Server, root, "Albums");
      var genres = new SimpleKeyedVirtualFolder(Server, root, "Genre");
      var folders = new VirtualFolder(Server, root, "Folders");
      SortFolder(Server, root, artists, performers, albums, genres);
      foreach (var f in root.ChildFolders.ToList()) {
        folders.AdoptItem(f as IFileServerMediaItem);
      }
      root.AdoptItem(artists);
      Server.RegisterPath(artists);
      root.AdoptItem(performers);
      Server.RegisterPath(performers);
      root.AdoptItem(albums);
      Server.RegisterPath(albums);
      root.AdoptItem(genres);
      Server.RegisterPath(genres);
      root.AdoptItem(folders);
      Server.RegisterPath(folders);
    }

    private void LinkTriple(TripleKeyedVirtualFolder folder, BaseFile r, string key1, string key2)
    {
      if (string.IsNullOrWhiteSpace(key1)) {
        return;
      }
      if (string.IsNullOrWhiteSpace(key2)) {
        return;
      }
      folder
        .GetFolder(key1.TrimStart().First().ToString().ToUpper())
        .GetFolder(key1)
        .GetFolder(key2)
        .Link(r);
    }

    private void SortFolder(FileServer server, IFileServerFolder folder, TripleKeyedVirtualFolder artists, TripleKeyedVirtualFolder performers, DoubleKeyedVirtualFolder albums, SimpleKeyedVirtualFolder genres)
    {
      foreach (var f in folder.ChildFolders.ToList()) {
        SortFolder(server, f as IFileServerFolder, artists, performers, albums, genres);
      }
      foreach (var i in folder.ChildItems.ToList()) {
        var ai = i as AudioFile;
        if (ai == null) {
          continue;
        }
        var album = ai.MetaAlbum;
        if (album == null) {
          album = "Unspecified album";
        }
        albums.GetFolder(album.TrimStart().First().ToString().ToUpper()).GetFolder(album).Link(ai);
        LinkTriple(artists, ai, ai.MetaArtist, album);
        LinkTriple(performers, ai, ai.MetaPerformer, album);
        var genre = ai.MetaGenre;
        if (genre != null) {
          genres.GetFolder(genre).Link(ai);
        }
      }
    }
  }
}
