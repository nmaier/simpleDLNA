using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using log4net;
using log4net.Core;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;

namespace NMaier.SimpleDlna.GUI
{
  internal class ServerListViewItem : ListViewItem, IDisposable
  {
    private readonly FileInfo cacheFile;

    internal readonly ServerDescription Description;

    private readonly HttpServer server;

    private FileServer fileServer;

    private State internalState = State.Idle;

    internal ServerListViewItem(HttpServer server, FileInfo cacheFile, ServerDescription description)
    {
      this.server = server;
      this.cacheFile = cacheFile;
      Description = description;

      Text = Description.Name;
      SubItems.Add(Description.Directories.Length.ToString());
      SubItems.Add(internalState.ToString());
      ImageIndex = 0;
    }

    private State InternalState
    {
      get { return internalState; }
      set {
        internalState = value;
        UpdateInfo();
      }
    }

    public void Dispose()
    {
      if (fileServer != null) {
        fileServer.Dispose();
        fileServer = null;
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
              ((ColumnHeader)c).AutoResize(mode);
            }
          }
        }
      }));
    }

    private void StartFileServer()
    {
      if (!Description.Active) {
        InternalState = State.Stopped;
        return;
      }
      var start = DateTime.Now;
      try {
        InternalState = State.Loading;
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
        fileServer = new FileServer(Description.Types, ids, dirs)
        {
          FriendlyName = Description.Name
        };
#if !DEBUG
        if (cacheFile != null) {
          fileServer.SetCacheFile(cacheFile);
        }
#endif
        fileServer.Changing += (o, e) => { InternalState = State.Refreshing; };
        fileServer.Changed += (o, e) => { InternalState = Description.Active ? State.Running : State.Stopped; };
        fileServer.Load();
        var authorizer = new HttpAuthorizer();
        if (Description.Ips.Length != 0) {
          authorizer.AddMethod(new IPAddressAuthorizer(Description.Ips));
        }
        if (Description.Macs.Length != 0) {
          authorizer.AddMethod(new MacAuthorizer(Description.Macs));
        }
        if (Description.UserAgents.Length != 0) {
          authorizer.AddMethod(new UserAgentAuthorizer(Description.UserAgents));
        }
        fileServer.Authorizer = authorizer;
        server.RegisterMediaServer(fileServer);
        InternalState = State.Running;
        var elapsed = DateTime.Now - start;
        LogManager.GetLogger("State").Logger.Log(
          GetType(),
          Level.Notice,
          $"{fileServer.FriendlyName} loaded in {elapsed.TotalSeconds:F2} seconds",
          null
          );
      }
      catch (Exception ex) {
        server.ErrorFormat("Failed to start {0}, {1}", Description.Name, ex);
        Description.ToggleActive();
        InternalState = State.Stopped;
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

      InternalState = State.Stopped;
    }

    private void UpdateInfo()
    {
      BeginInvoke(() =>
      {
        SubItems.Clear();

        Text = Description.Name;
        SubItems.Add(Description.Directories.Length.ToString());
        SubItems.Add(InternalState.ToString());
        ImageIndex = (int)InternalState;
      });
    }

    internal void Load()
    {
      InternalState = State.Loading;
      StartFileServer();
    }

    internal void Rescan()
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

    internal void Toggle()
    {
      StopFileServer();
      Description.ToggleActive();
      StartFileServer();
    }

    internal void UpdateInfo(ServerDescription description)
    {
      StopFileServer();
      Description.AdoptInfo(description);
      StartFileServer();
    }

    private enum State
    {
      Idle = 0,
      Running = 1,
      Stopped = 2,
      Refreshing = 3,
      Loading = 4
    }
  }
}
