using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;

namespace NMaier.SimpleDlna.GUI
{
  internal class ServerListViewItem : ListViewItem, IDisposable
  {
    private readonly FileInfo cacheFile = null;

    private FileServer fileServer;

    private readonly HttpServer server;


    public readonly ServerDescription Description;


    public ServerListViewItem(HttpServer server, FileInfo cacheFile, ServerDescription description)
    {
      this.server = server;
      this.cacheFile = cacheFile;
      Description = description;
      StartFileServer();
      UpdateInfo();
    }


    private void StartFileServer()
    {
      if (!Description.Active) {
        return;
      }
      try {
        var ids = new Identifiers(ComparerRepository.Lookup(Description.Order), Description.OrderDescending);
        foreach (var v in Description.Views) {
          ids.AddView(v);
        }
        var dirs = (from i in Description.Directories
                    let d = new DirectoryInfo(i)
                    where d.Exists
                    select d).ToArray();
        if (dirs.Length == 0) {
          throw new InvalidOperationException("No remaining directories");
        }
        fileServer = new FileServer(Description.Types, ids, dirs);
        if (cacheFile != null) {
          fileServer.SetCacheFile(cacheFile);
        }
        fileServer.Load();
        server.RegisterMediaServer(fileServer);
      }
      catch (Exception ex) {
        server.ErrorFormat("Failed to start {0}, {1}", Description.Name, ex);
        Description.ToggleActive();
      }
    }

    private void StopFileServer()
    {
      if (!Description.Active || fileServer == null) {
        return;
      }
      server.UnregisterMediaServer(fileServer);
      fileServer.Dispose();
      fileServer = null;
    }

    private void UpdateInfo()
    {
      SubItems.Clear();

      Text = Description.Name;
      SubItems.Add(Description.Directories.Length.ToString());
      SubItems.Add(Description.Active ? "Active" : "Inactive");
    }


    public void Dispose()
    {
      if (fileServer != null) {
        fileServer.Dispose();
        fileServer = null;
      }
    }

    public void Toggle()
    {
      StopFileServer();
      Description.ToggleActive();
      StartFileServer();
      UpdateInfo();
    }

    public void UpdateInfo(ServerDescription description)
    {
      StopFileServer();
      Description.AdoptInfo(description);
      StartFileServer();
      UpdateInfo();
    }
  }
}
