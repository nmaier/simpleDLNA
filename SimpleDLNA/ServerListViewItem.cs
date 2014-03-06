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

    private State internalState = State.Loading;

    private readonly HttpServer server;


    public readonly ServerDescription Description;


    public ServerListViewItem(HttpServer server, FileInfo cacheFile, ServerDescription description)
    {
      this.server = server;
      this.cacheFile = cacheFile;
      Description = description;

      Text = Description.Name;
      SubItems.Add(Description.Directories.Length.ToString());
      ImageIndex = 0;
    }


    private enum State : int
    {
      Loading = 0,
      Refreshing = 3,
      Running = 1,
      Stopped = 2
    }


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


    private void BeginInvoke(Action func)
    {
      ListView.BeginInvoke((Action)(() =>
      {
        try {
          func();
        }
        finally {
          if (ListView != null) {
            var mode = ListView.Items.Count == 0
              ? ColumnHeaderAutoResizeStyle.HeaderSize
              : ColumnHeaderAutoResizeStyle.ColumnContent;
            foreach (var c in ListView.Columns) {
              (c as ColumnHeader).AutoResize(mode);
            }
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
        fileServer = new FileServer(Description.Types, ids, dirs) { FriendlyName = Description.Name };
#if !DEBUG
        if (cacheFile != null) {
          fileServer.SetCacheFile(cacheFile);
        }
#endif
        fileServer.Changing += (o, e) =>
        {
          state = State.Refreshing;
        };
        fileServer.Changed += (o, e) =>
        {
          state = Description.Active ? State.Running : State.Stopped;
        };
        fileServer.Load();
        var authorizer = new HttpAuthorizer();
        if (Description.Ips.Length != 0) {
          authorizer.AddMethod(new IpAuthorizer(Description.Ips));
        }
        if (Description.Macs.Length != 0) {
          authorizer.AddMethod(new MacAuthorizer(Description.Macs));
        }
        if (Description.UserAgents.Length != 0) {
          authorizer.AddMethod(new UserAgentAuthorizer(Description.UserAgents));
        }
        fileServer.Authorizer = authorizer;
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

    public void Rescan()
    {
      if (fileServer == null) {
        throw new ArgumentException("Server is not running");
      }
      var vs = fileServer as IVolatileMediaServer;
      if (vs == null) {
        throw new ArgumentException("Server does not support rescanning");
      }
      vs.Rescan();
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
