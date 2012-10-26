using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;
using NMaier.sdlna.Util;

namespace NMaier.sdlna.Server
{
  public sealed class HttpServer : Logging, IDisposable
  {

    private readonly Dictionary<HttpClient, DateTime> clients = new Dictionary<HttpClient, DateTime>();
    private readonly TcpListener listener;
    private readonly Dictionary<string, IPrefixHandler> prefixes = new Dictionary<string, IPrefixHandler>();
    public static readonly string SERVER_SIGNATURE = GenerateServerSignature();
    private readonly Dictionary<Guid, MediaMount> servers = new Dictionary<Guid, MediaMount>();
    private readonly SSDPServer ssdpServer;
    private readonly Timer timeouter = new Timer(10 * 1000);



    public HttpServer(int port = 0)
    {
      listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
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

    public void RegisterMediaServer(IMediaServer aServer)
    {
      var guid = aServer.UUID;
      if (servers.ContainsKey(guid)) {
        throw new ArgumentException("Attempting to register more than once");
      }

      var end = listener.LocalEndpoint as IPEndPoint;
      var mount = new MediaMount(aServer);
      servers[guid] = mount;
      RegisterHandler(mount);

      try {

        foreach (var adapter in NetworkInterface.GetAllNetworkInterfaces()) {
          foreach (var uni in adapter.GetIPProperties().UnicastAddresses) {
            var address = uni.Address;
            if (address.AddressFamily != AddressFamily.InterNetwork || IPAddress.IsLoopback(address)) {
              continue;
            }
            var uri = new Uri(string.Format("http://{0}:{1}{2}", address, end.Port, mount.DescriptorURI));
            ssdpServer.RegisterNotification(guid, uri);
            InfoFormat("New mount at: {0}", uri);
          }
        }
      }
      catch (Exception ex) {
        Error("Failed to retrieve IP addresses the usual way, falling back to naive mode", ex);
        var uri = new Uri(string.Format("http://{0}:{1}{2}", GetIP(), end.Port, mount.DescriptorURI));
        ssdpServer.RegisterNotification(guid, uri);
        InfoFormat("New naive mount at: {0}", uri);
      }
    }

    public void UnregisterMediaServer(IMediaServer aServer)
    {
      MediaMount mount;
      if (!servers.TryGetValue(aServer.UUID, out mount)) {
        return;
      }

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
      TcpClient tcpclient = null;
      try {
        tcpclient = listener.EndAcceptTcpClient(result);
      }
      catch (Exception ex) {
        Error("Failed to accept a client", ex);
      }
      Accept();
      try {
        var client = new HttpClient(this, tcpclient);
        lock (clients) {
          clients.Add(client, DateTime.Now);
        }
        DebugFormat("Accepted client {0}", client);
        client.Start();
      }
      catch (Exception ex) {
        Error("Failed to accept a client", ex);
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
          DebugFormat("Collected timeout client {0}", c);
          c.Key.Close();
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

    internal void RemoveClient(HttpClient client)
    {
      if (!clients.ContainsKey(client)) {
        return;
      }
      clients.Remove(client);
    }

    internal void UnregisterHandler(IPrefixHandler handler)
    {
      prefixes.Remove(handler.Prefix);
      DebugFormat("Unregistered Handler for {0}", handler.Prefix);
    }
  }
}
