using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Timers;

namespace NMaier.sdlna.Server
{
  public class HttpServer : Logging, IDisposable
  {

    private Dictionary<HttpClient, DateTime> clients = new Dictionary<HttpClient, DateTime>();
    private TcpListener listener = new TcpListener(new IPEndPoint(IPAddress.Any, 0));
    private Dictionary<string, IPrefixHandler> prefixes = new Dictionary<string, IPrefixHandler>();
    private Dictionary<Guid, MediaMount> servers = new Dictionary<Guid, MediaMount>();
    public static string SERVER_SIGNATURE = GenerateServerSignature();
    private readonly SSDPServer ssdpServer;
    private readonly Timer timeouter = new Timer(100000);



    public HttpServer()
    {
      ssdpServer = new SSDPServer(this);
      timeouter.Elapsed += TimeouterCallback;
      timeouter.Enabled = true;

      prefixes.Add("/favicon.ico", new StaticHandler(new ResourceResponse(HttpCodes.OK, "image/icon", "favicon")));
      RegisterHandler(new IconHandler());

      listener.Server.Ttl = 32;
      listener.Start();
      InfoFormat("Running HTTP Server: {0} on port {1}", SERVER_SIGNATURE, (listener.LocalEndpoint as IPEndPoint).Port);
      Accept();
    }



    public static string ServerSignature
    {
      get
      {
        return SERVER_SIGNATURE;
      }
    }

    public string Signature { get { return SERVER_SIGNATURE; } }




    public void Dispose()
    {
      Debug("Disposing HTTP");
      timeouter.Enabled = false;
      foreach (var s in servers.Values.ToList()) {
        UnregisterMediaServer(s);
      }
      ssdpServer.Dispose();
    }

    internal void RegisterHandler(IPrefixHandler handler)
    {
      if (handler == null) {
        throw new ArgumentNullException();
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

    public void RegisterMediaServer(IMediaServer aServer)
    {
      var guid = aServer.UUID;
      if (servers.ContainsKey(guid)) {
        throw new ArgumentException("Attempting to register more than once");
      }

      var end = listener.LocalEndpoint as IPEndPoint;
      var baseURI = String.Format("http://{0}:{1}", GetIP(), end.Port);
      var mount = new MediaMount(aServer, baseURI);
      servers[guid] = mount;
      RegisterHandler(mount);


      var uri = new Uri(String.Format("{0}{1}", baseURI, mount.DescriptorURI));
      ssdpServer.RegisterNotification(guid, uri);
      InfoFormat("Registered Media Server {0}", aServer.UUID);
      InfoFormat("New mount at: {0}", uri);
    }

    internal void UnregisterHandler(IPrefixHandler handler)
    {
      prefixes.Remove(handler.Prefix);
      DebugFormat("Unregistered Handler for {0}", handler.Prefix);
    }

    public void UnregisterMediaServer(IMediaServer aServer)
    {
      MediaMount mount;
      if (!servers.TryGetValue(aServer.UUID, out mount)) {
        return;
      }
      var end = listener.LocalEndpoint as IPEndPoint;
      var uri = new Uri(String.Format("http://{0}:{1}{2}", GetIP(), end.Port, mount.DescriptorURI));
      ssdpServer.UnregisterNotification(aServer.UUID);
      UnregisterHandler(mount);
      servers.Remove(aServer.UUID);
      InfoFormat("Unregistered Media Server {0}", aServer.UUID);
    }

    private void Accept()
    {
      try {
        listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), null);
      }
      catch (Exception ex) {
        Fatal("Failed to accept", ex);
      }
    }

    private void AcceptCallback(IAsyncResult result)
    {
      try {
        var client = new HttpClient(this, listener.EndAcceptTcpClient(result));
        clients.Add(client, DateTime.Now);
        DebugFormat("Accepted client {0}", client);
      }
      catch (Exception ex) {
        Error("Failed to accept a client", ex);
      }

      Accept();
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

    private static IPAddress GetIP()
    {
      foreach (var i in Dns.GetHostEntry(Dns.GetHostName()).AddressList) {
        if (i.AddressFamily == AddressFamily.InterNetwork) {
          return i;
        }
      }
      throw new ApplicationException("No IP");
    }

    private void TimeouterCallback(object sender, ElapsedEventArgs e)
    {
      Debug("Timeouter");
      foreach (var c in clients.ToList()) {
        if (c.Key.IsATimeout) {
          c.Key.Close();
          DebugFormat("Collected timeout client {0}", c);
        }
      }
    }

    internal IPrefixHandler FindHandler(string prefix)
    {
      foreach (var s in prefixes.Keys) {
        if (prefix.StartsWith(s)) {
          return prefixes[s];
        }
      }
      return null;
    }

    internal void RemoveClient(HttpClient client)
    {
      if (!clients.ContainsKey(client)) {
        return;
      }
      clients.Remove(client);
    }
  }
}
