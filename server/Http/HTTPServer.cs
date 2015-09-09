﻿using NMaier.SimpleDlna.Server.Ssdp;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;

namespace NMaier.SimpleDlna.Server.Http
{//Logging, 
  public sealed class HttpServer : IDisposable
  {

    private static readonly ILogging _logger = Logging.GetLogger<HttpServer>();
    public ILogging Logger{get{return _logger;} }

    private readonly ConcurrentDictionary<HttpClient, DateTime> clients =
      new ConcurrentDictionary<HttpClient, DateTime>();

    private readonly ConcurrentDictionary<string, IPrefixHandler> prefixes =
      new ConcurrentDictionary<string, IPrefixHandler>();

    private readonly ConcurrentDictionary<Guid, List<Guid>> devicesForServers =
      new ConcurrentDictionary<Guid, List<Guid>>();

    private readonly ConcurrentDictionary<Guid, MediaMount> servers =
      new ConcurrentDictionary<Guid, MediaMount>();

    public static readonly string Signature = GenerateServerSignature();

    private readonly Timer timeouter = new Timer(10 * 1000);

    private readonly TcpListener listener = null;

    private readonly SsdpHandler ssdpServer;

    public HttpServer()
      : this(port: 0)
    {
    }

    public HttpServer(int port)
    {
      prefixes.TryAdd(
        "/favicon.ico",
        new StaticHandler(
          new ResourceResponse(HttpCode.Ok, "image/icon", "favicon"))
        );
      prefixes.TryAdd(
        "/static/browse.css",
        new StaticHandler(
          new ResourceResponse(HttpCode.Ok, "text/css", "browse_css"))
        );
      RegisterHandler(new IconHandler());

      listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
      listener.Server.Ttl = 32;
      listener.Server.UseOnlyOverlappedIO = true;
      listener.Start();

      RealPort = (listener.LocalEndpoint as IPEndPoint).Port;

      Logger.NoticeFormat(
        "Running HTTP Server: {0} on port {1}", Signature, RealPort);
      ssdpServer = new SsdpHandler();

      timeouter.Elapsed += TimeouterCallback;
      timeouter.Enabled = true;

      Accept();
    }

    public event EventHandler<HttpAuthorizationEventArgs> OnAuthorizeClient;

    public Dictionary<string, string> MediaMounts
    {
      get
      {
        var rv = new Dictionary<string, string>();
        foreach (var m in servers) {
          rv[m.Value.Prefix] = m.Value.FriendlyName;
        }
        return rv;
      }
    }

    public int RealPort { get; private set; }

    private void Accept()
    {
      try {
        if (!listener.Server.IsBound) {
          return;
        }
        listener.BeginAcceptTcpClient(AcceptCallback, null);
      }
      catch (ObjectDisposedException) {
      }
      catch (Exception ex) {
        Logger.Fatal("Failed to accept", ex);
      }
    }

    private void AcceptCallback(IAsyncResult result)
    {
      try {
        var tcpclient = listener.EndAcceptTcpClient(result);
        var client = new HttpClient(this, tcpclient);
        try {
          clients.AddOrUpdate(client, DateTime.Now, (k, v) =>
          {
            return DateTime.Now;
          });
          Logger.DebugFormat("Accepted client {0}", client);
          client.Start();
        }
        catch (Exception) {
          client.Dispose();
          throw;
        }
      }
      catch (ObjectDisposedException) {
      }
      catch (Exception ex) {
        Logger.Error("Failed to accept a client", ex);
      }
      finally {
        Accept();
      }
    }

    private static string GenerateServerSignature()
    {
      var os = Environment.OSVersion;
      var pstring = os.Platform.ToString();
      switch (os.Platform) {
        case PlatformID.Win32NT:
        case PlatformID.Win32S:
        case PlatformID.Win32Windows:
          pstring = "WIN";
          break;
        default:
          try {
            pstring = Formatting.GetSystemName();
          }
          catch (Exception ex) {
            _logger.Debug("Failed to get uname", ex);
          }
          break;
      }
      return String.Format(
      "{0}{1}/{2}.{3} UPnP/1.0 DLNADOC/1.5 sdlna/{4}.{5}",
      pstring,
      IntPtr.Size * 8,
      os.Version.Major,
      os.Version.Minor,
      Assembly.GetExecutingAssembly().GetName().Version.Major,
      Assembly.GetExecutingAssembly().GetName().Version.Minor
      );
    }

    private void TimeouterCallback(object sender, ElapsedEventArgs e)
    {
      foreach (var c in clients.ToList()) {
        if (c.Key.IsATimeout) {
          Logger.DebugFormat("Collected timeout client {0}", c);
          c.Key.Close();
        }
      }
    }

    internal bool AuthorizeClient(HttpClient client)
    {
      if (OnAuthorizeClient == null) {
        return true;
      }
      if (IPAddress.IsLoopback(client.RemoteEndpoint.Address)) {
        return true;
      }
      var e = new HttpAuthorizationEventArgs(
        client.Headers, client.RemoteEndpoint);
      OnAuthorizeClient(this, e);
      return !e.Cancel;
    }

    internal IPrefixHandler FindHandler(string prefix)
    {
      if (string.IsNullOrEmpty(prefix)) {
        throw new ArgumentNullException("prefix");
      }

      if (prefix == "/") {
        return new IndexHandler(this);
      }

      foreach (var s in prefixes.Keys) {
        if (prefix.StartsWith(s, StringComparison.Ordinal)) {
          return prefixes[s];
        }
      }
      return null;
    }

    internal void RegisterHandler(IPrefixHandler handler)
    {
      if (handler == null) {
        throw new ArgumentNullException("handler");
      }
      var prefix = handler.Prefix;
      if (!prefix.StartsWith("/", StringComparison.Ordinal)) {
        throw new ArgumentException("Invalid prefix; must start with /");
      }
      if (!prefix.EndsWith("/", StringComparison.Ordinal)) {
        throw new ArgumentException("Invalid prefix; must end with /");
      }
      if (FindHandler(prefix) != null) {
        throw new ArgumentException("Invalid prefix; already taken");
      }
      if (!prefixes.TryAdd(prefix, handler)) {
        throw new ArgumentException("Invalid preifx; already taken");
      }
      Logger.DebugFormat("Registered Handler for {0}", prefix);
    }

    internal void RemoveClient(HttpClient client)
    {
      DateTime ignored;
      clients.TryRemove(client, out ignored);
    }

    internal void UnregisterHandler(IPrefixHandler handler)
    {
      IPrefixHandler ignored;
      if (prefixes.TryRemove(handler.Prefix, out ignored)) {
        Logger.DebugFormat("Unregistered Handler for {0}", handler.Prefix);
      }
    }

    public void Dispose()
    {
      Logger.Debug("Disposing HTTP");
      timeouter.Enabled = false;
      foreach (var s in servers.Values.ToList()) {
        UnregisterMediaServer(s);
      }
      ssdpServer.Dispose();
      timeouter.Dispose();
      listener.Stop();
      foreach (var c in clients.ToList()) {
        c.Key.Dispose();
      }
      clients.Clear();
    }

    public void RegisterMediaServer(IMediaServer server)
    {
      if (server == null) {
        throw new ArgumentNullException("server");
      }
      var guid = server.Uuid;
      if (servers.ContainsKey(guid)) {
        throw new ArgumentException("Attempting to register more than once");
      }

      var end = listener.LocalEndpoint as IPEndPoint;
      var mount = new MediaMount(server);
      servers[guid] = mount;
      RegisterHandler(mount);

      foreach (var address in IP.ExternalIPAddresses) {
        Logger.DebugFormat("Registering device for {0}", address);
        var deviceGuid = Guid.NewGuid();
        var list = devicesForServers.GetOrAdd(guid, new List<Guid>());
        lock (list) {
          list.Add(deviceGuid);
        }
        mount.AddDeviceGuid(deviceGuid, address);
        var uri = new Uri(string.Format(
          "http://{0}:{1}{2}",
          address,
          end.Port,
          mount.DescriptorURI
          ));
        ssdpServer.RegisterNotification(deviceGuid, uri, address);
        Logger.NoticeFormat("New mount at: {0}", uri);
      }
    }

    public void UnregisterMediaServer(IMediaServer server)
    {
      if (server == null) {
        throw new ArgumentNullException("server");
      }
      MediaMount mount;
      if (!servers.TryGetValue(server.Uuid, out mount)) {
        return;
      }

      List<Guid> list;
      if (devicesForServers.TryGetValue(server.Uuid, out list)) {
        lock (list) {
          foreach (var deviceGuid in list) {
            ssdpServer.UnregisterNotification(deviceGuid);
          }
        }
        devicesForServers.TryRemove(server.Uuid, out list);
      }

      UnregisterHandler(mount);

      MediaMount ignored;
      if (servers.TryRemove(server.Uuid, out ignored)) {
        Logger.InfoFormat("Unregistered Media Server {0}", server.Uuid);
      }
    }
  }
}
