using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;
using NMaier.SimpleDlna.Server.Ssdp;
using NMaier.SimpleDlna.Utilities;

[assembly: CLSCompliant(true)]
namespace NMaier.SimpleDlna.Server
{
  public sealed class HttpServer : Logging, IDisposable
  {
    private readonly Dictionary<HttpClient, DateTime> clients = new Dictionary<HttpClient, DateTime>();

    private readonly Dictionary<string, IPrefixHandler> prefixes = new Dictionary<string, IPrefixHandler>();

    private readonly Dictionary<Guid, MediaMount> servers = new Dictionary<Guid, MediaMount>();

    public static readonly string Signature = GenerateServerSignature();

    private readonly Timer timeouter = new Timer(10 * 1000);

    private readonly TcpListener listener;

    private readonly SsdpHandler ssdpServer;


    public HttpServer()
      : this(port: 0)
    {
    }
    public HttpServer(int port)
    {
      listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
      timeouter.Elapsed += TimeouterCallback;
      timeouter.Enabled = true;

      prefixes.Add("/favicon.ico", new StaticHandler(new ResourceResponse(HttpCodes.OK, "image/icon", "favicon")));
      prefixes.Add("/static/browse.css", new StaticHandler(new ResourceResponse(HttpCodes.OK, "text/css", "browse_css")));
      RegisterHandler(new IconHandler());

      listener.Server.Ttl = 32;
      listener.Server.UseOnlyOverlappedIO = true;
      listener.Start();
      RealPort = (listener.LocalEndpoint as IPEndPoint).Port;
      InfoFormat("Running HTTP Server: {0} on port {1}", Signature, RealPort);
      ssdpServer = new SsdpHandler();
      Accept();
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
        Fatal("Failed to accept", ex);
      }
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
    private void AcceptCallback(IAsyncResult result)
    {
      try {
        var tcpclient = listener.EndAcceptTcpClient(result);
        var client = new HttpClient(this, tcpclient);
        try {
          lock (clients) {
            clients.Add(client, DateTime.Now);
          }
          DebugFormat("Accepted client {0}", client);
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
        Error("Failed to accept a client", ex);
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
      Debug("Timeouter");
      lock (clients) {
        foreach (var c in clients.ToList()) {
          if (c.Key.IsATimeout) {
            DebugFormat("Collected timeout client {0}", c);
            c.Key.Close();
          }
        }
      }
    }


    internal IPrefixHandler FindHandler(string prefix)
    {
      if (prefix == "/") {
        return new IndexHandler(this);
      }

      foreach (var s in prefixes.Keys) {
        if (prefix.StartsWith(s)) {
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
      if (!prefix.StartsWith("/")) {
        throw new ArgumentException("Invalid prefix; must start with /");
      }
      if (!prefix.EndsWith("/")) {
        throw new ArgumentException("Invalid prefix; must end with /");
      }
      if (FindHandler(prefix) != null) {
        throw new ArgumentException("Invalid prefix; already taken /");
      }
      prefixes.Add(prefix, handler);
      DebugFormat("Registered Handler for {0}", prefix);
    }

    internal void RemoveClient(HttpClient client)
    {
      lock (clients) {
        if (!clients.ContainsKey(client)) {
          return;
        }
        clients.Remove(client);
      }
    }

    internal void UnregisterHandler(IPrefixHandler handler)
    {
      prefixes.Remove(handler.Prefix);
      DebugFormat("Unregistered Handler for {0}", handler.Prefix);
    }


    public void Dispose()
    {
      Debug("Disposing HTTP");
      timeouter.Enabled = false;
      foreach (var s in servers.Values.ToList()) {
        UnregisterMediaServer(s);
      }
      ssdpServer.Dispose();
      timeouter.Dispose();
      listener.Stop();
      lock (clients) {
        foreach (var c in clients.ToList()) {
          c.Key.Dispose();
        }
        clients.Clear();
      }
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
        var uri = new Uri(string.Format("http://{0}:{1}{2}", address, end.Port, mount.DescriptorURI));
        ssdpServer.RegisterNotification(guid, uri);
        InfoFormat("New mount at: {0}", uri);
      }
    }

    public Dictionary<string, string> MediaMounts
    {
      get
      {
        Dictionary<string, string> rv = new Dictionary<string, string>();
        foreach (var m in servers) {
          rv[m.Value.Prefix] = m.Value.FriendlyName;
        }
        return rv;
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

      ssdpServer.UnregisterNotification(server.Uuid);
      UnregisterHandler(mount);
      servers.Remove(server.Uuid);
      InfoFormat("Unregistered Media Server {0}", server.Uuid);
    }
  }
}
