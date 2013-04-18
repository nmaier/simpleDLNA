using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal class PlainFolder : VirtualFolder, IMetaInfo
  {
    private readonly DirectoryInfo dir;


    public PlainFolder(FileServer server, DlnaMediaTypes types, VirtualFolder parent, DirectoryInfo dir)
      : base(parent, dir.Name)
    {
      Server = server;
      this.dir = dir;
      folders = (from d in dir.GetDirectories()
                 let m = new PlainFolder(server, types, this, d)
                 where m.ChildCount > 0
                 select m as IMediaFolder).ToList();

      foreach (var i in DlnaMaps.Media2Ext) {
        if (!types.HasFlag(i.Key)) {
          continue;
        }
        foreach (var ext in i.Value) {
          var _files = from f in dir.GetFiles("*." + ext)
                       select f;
          var files = new List<BaseFile>();
          foreach (var f in _files) {
            try {
              files.Add(server.GetFile(this, f));
            }
            catch (Exception ex) {
              server.Warn(f);
              server.Warn(ex);
            }
          }
          resources.AddRange(files);
        }
      }
    }


    public DateTime InfoDate
    {
      get
      {
        return dir.LastWriteTimeUtc;
      }
    }
    public long? InfoSize
    {
      get
      {
        return null;
      }
    }
    public override string Path
    {
      get
      {
        return dir.FullName;
      }
    }
    public FileServer Server
    {
      get;
      protected set;
    }
    public override string Title
    {
      get
      {
        return dir.Name;
      }
    }
  }
}
