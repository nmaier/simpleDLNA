using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NMaier.SimpleDlna.FileMediaServer.Files;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class PlainFolder : BaseFolder, IMetaInfo
  {

    private readonly DirectoryInfo dir;



    public PlainFolder(FileServer server, MediaTypes types, BaseFolder aParent, DirectoryInfo aDir)
      : base(server, aParent)
    {
      dir = aDir;
      childFolders = (from d in dir.GetDirectories()
                      let m = new PlainFolder(server, types, this, d)
                      where m.ChildCount > 0
                      select m as BaseFolder).ToList();

      childItems = new List<BaseFile>();
      foreach (var i in DlnaMaps.Media2Ext) {
        if (!types.HasFlag(i.Key)) {
          continue;
        }
        foreach (var ext in i.Value) {
          var files = (from f in dir.GetFiles("*." + ext)
                      let m = server.GetFile(this, f)
                      select m).ToList();
          childItems.AddRange(files);
        }
      }
    }



    public DateTime InfoDate
    {
      get { return dir.LastWriteTimeUtc; }
    }

    public long? InfoSize
    {
      get { return null; }
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