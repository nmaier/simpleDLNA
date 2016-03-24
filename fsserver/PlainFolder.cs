using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal class PlainFolder : VirtualFolder, IMetaInfo
  {
    private readonly DirectoryInfo dir;

    internal PlainFolder(FileServer server, VirtualFolder parent, DirectoryInfo dir)
      : base(parent, dir.Name)
    {
      Server = server;
      this.dir = dir;
      var rawfiles = (from f in dir.GetFiles("*.*")
                      select f);
      var files = new List<BaseFile>();
      foreach (var f in rawfiles) {
        var ext = f.Extension;
        if (string.IsNullOrEmpty(ext) ||
          !server.Filter.Filtered(ext.Substring(1))) {
          continue;
        }
        try {
          var file = server.GetFile(this, f);
          if (server.Allowed(file)) {
            files.Add(file);
          }
        }
        catch (Exception ex) {
          server.Warn(f, ex);
        }
      }
      resources.AddRange(files);

      folders = (from d in dir.GetDirectories()
                 let m = TryGetFolder(server, d)
                 where m != null && m.ChildCount > 0
                 select m as IMediaFolder).ToList();
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

    private PlainFolder TryGetFolder(FileServer server, DirectoryInfo d)
    {
      try {
        return new PlainFolder(server, this, d);
      }
      catch (Exception ex) {
        server.Warn("Failed to access folder", ex);
        return null;
      }
    }
  }
}
