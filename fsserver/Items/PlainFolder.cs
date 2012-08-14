using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class PlainFolder : AbstractFolder, IMediaItemMetaData
  {

    private readonly DirectoryInfo dir;



    public PlainFolder(FileServer server, MediaTypes types, IMediaFolder aParent, DirectoryInfo aDir)
      : base(server, aParent)
    {
      dir = aDir;
      childFolders = (from d in dir.GetDirectories()
                      let m = new PlainFolder(server, types, this, d)
                      where m.ChildCount > 0
                      select m as IFileServerFolder).ToList();

      var files = new List<IFileServerResource>().AsEnumerable();
      foreach (var i in DlnaMaps.Media2Ext) {
        if (!types.HasFlag(i.Key)) {
          continue;
        }
        foreach (var ext in i.Value) {
          files = files.Union(from f in dir.GetFiles("*." + ext)
                              let m = new File(this, f)
                              select m as IFileServerResource);
        }
      }
      childItems = files.ToList();
      foreach (var c in childFolders) {
        server.RegisterPath(c);
      }
      foreach (var c in childItems) {
        server.RegisterPath(c);
      }
    }



    public DateTime ItemDate
    {
      get { return dir.LastWriteTimeUtc; }
    }

    public long ItemSize
    {
      get { throw new NotSupportedException(); }
    }

    public override string Path
    {
      get { return dir.FullName; }
    }

    public override string Title
    {
      get { return dir.Name; }
    }
  }
}