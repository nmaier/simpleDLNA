using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Timers;
using NMaier.SimpleDlna.Utilities;
using NMaier.SimpleDlna.Server.Ssdp;

namespace NMaier.SimpleDlna.Server
{
  public sealed class HttpServer : Logging, IDisposable
  {

    private readonly Dictionary<HttpClient, DateTime> clients = new Dictionary<HttpClient, DateTime>();
    private readonly TcpListener listener;
    private readonly Dictionary<string, IPrefixHandler> prefixes = new Dictionary<string, IPrefixHandler>();
    private readonly Dictionary<Guid, MediaMount> servers = new Dictionary<Guid, MediaMount>();
    public static readonly string Signature = GenerateServerSignature();
    private readonly SsdpHandler ssdpServer;
    private readonly Timer timeouter = new Timer(10 * 1000);



    public HttpServer(int port = 0)
    {
      listener = new TcpListener(new IPEndPoint(IPAddress.Any, port));
      timeouter.Elapsed += TimeouterCallback;
      timeouter.Enabled = true;

      prefixes.Add("/favicon.ico", new StaticHandler(new ResourceResponse(HttpCodes.OK, "image/icon", "favicon")));
      RegisterHandler(new IconHandler());

      listener.Server.Ttl = 32;
      listener.Server.UseOnlyOverlappedIO = true;
      listener.Start();
      var realPort = (listener.LocalEndpoint as IPEndPoint).Port;
      InfoFormat("Running HTTP Server: {0} on port {1}", Signature, realPort);
      ssdpServer = new SsdpHandler(realPort);
      Accept();
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
      var guid = server.Uuid;
      if (servers.ContainsKey(guid)) {
        throw new ArgumentException("Attempting to register more than once");
      }

      var end = listener.LocalEndpoint as IPEndPoint;
      var mount = new MediaMount(server);
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

    public void UnregisterMediaServer(IMediaServer server)
    {
      MediaMount mount;
      if (!servers.TryGetValue(server.Uuid, out mount)) {
        return;
      }

      ssdpServer.UnregisterNotification(server.Uuid);
      UnregisterHandler(mount);
      servers.Remove(server.Uuid);
      InfoFormat("Unregistered Media Server {0}", server.Uuid);
    }

    private void Accept()
    {
      try {
        if (!listener.Server.IsBound) {
          return;
        }
        listener.BeginAcceptTcpClient(new AsyncCallback(AcceptCallback), null);
      }
      catch (ObjectDisposedException) { }
      catch (Exception ex) {
        Fatal("Failed to accept", ex);
      }
    }

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
      catch (ObjectDisposedException) { }
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
  }
}
