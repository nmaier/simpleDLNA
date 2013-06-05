using System.IO;
using System.Linq;
using System.Windows.Forms;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;

namespace NMaier.SimpleDlna.GUI
{
  internal class ServerListViewItem : ListViewItem
  {
    public readonly ServerDescription Description;
    private FileServer fileServer;

    private readonly HttpServer server;
    private readonly FileInfo cacheFile = null;

    public ServerListViewItem(HttpServer server, FileInfo cacheFile, ServerDescription description)
    {
      this.server = server;
      this.cacheFile = cacheFile;
      Description = description;
      UpdateInfo();
      StartFileServer();
    }

    public void Toggle()
    {
      StopFileServer();
      Description.Active = !Description.Active;
      UpdateInfo();
      StartFileServer();
    }

    public void UpdateInfo(ServerDescription description)
    {
      StopFileServer();
      Description.AdoptInfo(description);
      UpdateInfo();
      StartFileServer();
    }
    private void UpdateInfo()
    {
      SubItems.Clear();

      Text = Description.Name;
      SubItems.Add(Description.Directories.Length.ToString());
      SubItems.Add(Description.Active ? "Active" : "Inactive");
    }
    private void StartFileServer()
    {
      if (Description.Active) {
        var ids = new Identifiers(ComparerRepository.Lookup(Description.Order), Description.OrderDescending);
        foreach (var v in Description.Views) {
          ids.AddView(v);
        }
        var dirs = (from i in Description.Directories
                    let d = new DirectoryInfo(i)
                    where d.Exists
                    select d).ToArray();
        fileServer = new FileServer(Description.Types, ids, dirs);
        if (cacheFile != null) {
          fileServer.SetCacheFile(cacheFile);
        }
        fileServer.Load();
        server.RegisterMediaServer(fileServer);
      }
    }

    private void StopFileServer()
    {
      if (Description.Active && fileServer != null) {
        server.UnregisterMediaServer(fileServer);
        fileServer.Dispose();
        fileServer = null;
      }
    }
  }
}
