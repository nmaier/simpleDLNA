using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  internal class ServerListViewItem : ListViewItem, IDisposable
  {
    private readonly FileInfo cacheFile = null;

    private FileServer fileServer;

    private readonly HttpServer server;
    private State internalState;
    private State state
    {
      get
      {
        return internalState;
      }
      set
      {
        internalState = value;
        UpdateInfo();
      }
    }

    public readonly ServerDescription Description;


    public ServerListViewItem(HttpServer server, FileInfo cacheFile, ServerDescription description)
    {
      this.server = server;
      this.cacheFile = cacheFile;
      Description = description;
    }


    private enum State : int
    {
      Running = 1,
      Stopped = 2,
      Loading = 0,
      Refreshing = 3
    }


    private void BeginInvoke(Action func)
    {
      ListView.BeginInvoke((Action)(() => {
        try {
          func();
        }
        finally {
          var mode = ListView.Items.Count == 0
            ? ColumnHeaderAutoResizeStyle.HeaderSize
            : ColumnHeaderAutoResizeStyle.ColumnContent;
          foreach (var c in ListView.Columns) {
            (c as ColumnHeader).AutoResize(mode);
          }
        }
      }));
    }

    private void StartFileServer()
    {
      if (!Description.Active) {
        state = State.Stopped;
        return;
      }
      try {
        state = State.Loading;
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
        fileServer.FriendlyName = Description.Name;
#if !DEBUG
        if (cacheFile != null) {
          fileServer.SetCacheFile(cacheFile);
        }
#endif
        fileServer.Load();
        server.RegisterMediaServer(fileServer);
        state = State.Running;
      }
      catch (Exception ex) {
        server.ErrorFormat("Failed to start {0}, {1}", Description.Name, ex);
        Description.ToggleActive();
        state = State.Stopped;
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

      state = State.Stopped;
    }

    private void UpdateInfo()
    {
      BeginInvoke(() =>
      {
        SubItems.Clear();

        Text = Description.Name;
        SubItems.Add(Description.Directories.Length.ToString());
        SubItems.Add(state.ToString());
        ImageIndex = (int)state;
      });
    }


    public void Dispose()
    {
      if (fileServer != null) {
        fileServer.Dispose();
        fileServer = null;
      }
    }

    public void Load()
    {
      state = State.Loading;
      StartFileServer();
    }

    public void Toggle()
    {
      StopFileServer();
      Description.ToggleActive();
      StartFileServer();
    }

    public void UpdateInfo(ServerDescription description)
    {
      StopFileServer();
      Description.AdoptInfo(description);
      StartFileServer();
    }
  }
}
