﻿using NMaier.SimpleDlna.Server.Http;
using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Threading = System.Threading;
using Timers = System.Timers;

namespace NMaier.SimpleDlna.Server.Ssdp
{//Logging, 
  internal sealed class SsdpHandler : IDisposable
  {

    private static readonly ILogging _logger = Logging.GetLogger<SsdpHandler>();

    private const int DATAGRAMS_PER_MESSAGE = 3;

    private const string SSDP_ADDR = "239.255.255.250";

    private const int SSDP_PORT = 1900;

    private readonly UdpClient client = new UdpClient();

    private readonly Threading.AutoResetEvent datagramPosted =
      new Threading.AutoResetEvent(false);

    private readonly Dictionary<Guid, List<UpnpDevice>> devices =
      new Dictionary<Guid, List<UpnpDevice>>();

    private readonly ConcurrentQueue<Datagram> messageQueue =
      new ConcurrentQueue<Datagram>();

    private readonly Timers.Timer notificationTimer =
      new Timers.Timer(60000);

    private readonly Timers.Timer queueTimer =
      new Timers.Timer(1000);

    private static readonly Random random = new Random();

    private static readonly IPEndPoint SSDP_ENDP =
      new IPEndPoint(IPAddress.Parse(SSDP_ADDR), SSDP_PORT);

    internal static readonly IPEndPoint BROAD_ENDP =
      new IPEndPoint(IPAddress.Parse("255.255.255.255"), SSDP_PORT);

    private static readonly IPAddress SSDP_IP =
      IPAddress.Parse(SSDP_ADDR);

    private bool running = true;

    public SsdpHandler()
    {
      notificationTimer.Elapsed += Tick;
      notificationTimer.Enabled = true;

      queueTimer.Elapsed += ProcessQueue;

      client.Client.UseOnlyOverlappedIO = true;
      client.Client.SetSocketOption(
        SocketOptionLevel.Socket,
        SocketOptionName.ReuseAddress,
        true
        );
      client.ExclusiveAddressUse = false;
      client.Client.Bind(new IPEndPoint(IPAddress.Any, SSDP_PORT));
      client.JoinMulticastGroup(SSDP_IP, 10);
      _logger.Notice("SSDP service started");
      Receive();
    }

    private UpnpDevice[] Devices
    {
      get
      {
        UpnpDevice[] devs;
        lock (devices) {
          devs = devices.Values.SelectMany(i =>
          {
            return i;
          }).ToArray();
        }
        return devs;
      }
    }

    private void ProcessQueue(object sender, Timers.ElapsedEventArgs e)
    {
      while (messageQueue.Count != 0) {
        var msg = (Datagram)null;
        if (!messageQueue.TryPeek(out msg)) {
          continue;
        }
        if (msg != null && (running || msg.Sticky)) {
          msg.Send();
          if (msg.SendCount > DATAGRAMS_PER_MESSAGE) {
            messageQueue.TryDequeue(out msg);
          }
          break;
        }
        else {
          messageQueue.TryDequeue(out msg);
        }
      }
      datagramPosted.Set();
      queueTimer.Enabled = messageQueue.Count != 0;
      queueTimer.Interval = random.Next(25, running ? 75 : 50);
    }

    private void Receive()
    {
      try {
        client.BeginReceive(ReceiveCallback, null);
      }
      catch (ObjectDisposedException) {
      }
    }

    private void ReceiveCallback(IAsyncResult result)
    {
      try {
        var endpoint = new IPEndPoint(IPAddress.None, SSDP_PORT);
        var received = client.EndReceive(result, ref endpoint);
        if (received == null) {
          throw new IOException("Didn't receive anything");
        }
        if (received.Length == 0) {
          throw new IOException("Didn't receive any bytes");
        }
#if DUMP_ALL_SSDP
        DebugFormat("{0} - SSDP Received a datagram", endpoint);
#endif
        using (var reader = new StreamReader(
          new MemoryStream(received), Encoding.ASCII)) {
          var proto = reader.ReadLine();
          if (proto == null) {
            throw new IOException("Couldn't read protocol line");
          }
          proto = proto.Trim();
          if (string.IsNullOrEmpty(proto)) {
            throw new IOException("Invalid protocol line");
          }
          var method = proto.Split(new char[] { ' ' }, 2)[0];
          var headers = new Headers();
          for (var line = reader.ReadLine(); line != null;
            line = reader.ReadLine()) {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) {
              break;
            }
            var parts = line.Split(new char[] { ':' }, 2);
            headers[parts[0]] = parts[1].Trim();
          }
#if DUMP_ALL_SSDP
          DebugFormat("{0} - Datagram method: {1}", endpoint, method);
          Debug(headers);
#endif
          if (method == "M-SEARCH") {
            RespondToSearch(endpoint, headers["st"]);
          }
        }
      }
      catch (IOException ex) {
        _logger.Debug("Failed to read SSDP message", ex);
      }
      catch (Exception ex) {
        _logger.Warn("Failed to read SSDP message", ex);
      }
      Receive();
    }

    private void SendDatagram(IPEndPoint endpoint, IPAddress address,
                              string message, bool sticky)
    {
      if (!running) {
        return;
      }
      var dgram = new Datagram(endpoint, address, message, sticky);
      if (messageQueue.Count == 0) {
        dgram.Send();
      }
      messageQueue.Enqueue(dgram);
      queueTimer.Enabled = true;
    }

    private void SendSearchResponse(IPEndPoint endpoint, UpnpDevice dev)
    {
      var headers = new RawHeaders();
      headers.Add("CACHE-CONTROL", "max-age = 600");
      headers.Add("DATE", DateTime.Now.ToString("R"));
      headers.Add("EXT", string.Empty);
      headers.Add("LOCATION", dev.Descriptor.ToString());
      headers.Add("SERVER", HttpServer.Signature);
      headers.Add("ST", dev.Type);
      headers.Add("USN", dev.USN);

      SendDatagram(
        endpoint,
        dev.Address,
        String.Format("HTTP/1.1 200 OK\r\n{0}\r\n", headers.HeaderBlock),
        false
        );
      _logger.InfoFormat(
        "{2}, {1} - Responded to a {0} request", dev.Type, endpoint,
        dev.Address);
    }

    private void Tick(object sender, Timers.ElapsedEventArgs e)
    {
      _logger.Debug("Sending SSDP notifications!");
      notificationTimer.Interval = random.Next(60000, 120000);
      NotifyAll();
    }

    internal void NotifyAll()
    {
      _logger.Debug("NotifyAll");
      foreach (var d in Devices) {
        NotifyDevice(d, "alive", false);
      }
    }

    internal void NotifyDevice(UpnpDevice dev, string type, bool sticky)
    {
      _logger.Debug("NotifyDevice");
      var headers = new RawHeaders();
      headers.Add("HOST", "239.255.255.250:1900");
      headers.Add("CACHE-CONTROL", "max-age = 600");
      headers.Add("LOCATION", dev.Descriptor.ToString());
      headers.Add("SERVER", HttpServer.Signature);
      headers.Add("NTS", "ssdp:" + type);
      headers.Add("NT", dev.Type);
      headers.Add("USN", dev.USN);

      SendDatagram(
        SSDP_ENDP,
        dev.Address,
        String.Format("NOTIFY * HTTP/1.1\r\n{0}\r\n", headers.HeaderBlock),
        sticky
        );
      // Some buggy network equipment will swallow multicast packets, so lets
      // cheat, increase the odds, by sending to broadcast.
      SendDatagram(
        BROAD_ENDP,
        dev.Address,
        String.Format("NOTIFY * HTTP/1.1\r\n{0}\r\n", headers.HeaderBlock),
        sticky
        );
      _logger.DebugFormat("{0} said {1}", dev.USN, type);
    }

    internal void RegisterNotification(Guid UUID, Uri Descriptor,
                                       IPAddress address)
    {
      List<UpnpDevice> list;
      lock (devices) {
        if (!devices.TryGetValue(UUID, out list)) {
          devices.Add(UUID, list = new List<UpnpDevice>());
        }
      }
      foreach (var t in new string[] {
        "upnp:rootdevice",
        "urn:schemas-upnp-org:device:MediaServer:1",
        "urn:schemas-upnp-org:service:ContentDirectory:1",
        "urn:schemas-upnp-org:service:ConnectionManager:1",
        "urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1",
        "uuid:" + UUID
      }) {
        list.Add(new UpnpDevice(UUID, t, Descriptor, address));
      }

      NotifyAll();
      _logger.DebugFormat("Registered mount {0}, {1}", UUID, address);
    }

    internal void RespondToSearch(IPEndPoint endpoint, string req)
    {
      if (req == "ssdp:all") {
        req = null;
      }

      _logger.DebugFormat("RespondToSearch {0} {1}", endpoint, req);
      foreach (var d in Devices) {
        if (!string.IsNullOrEmpty(req) && req != d.Type) {
          continue;
        }
        SendSearchResponse(endpoint, d);
      }
    }

    internal void UnregisterNotification(Guid UUID)
    {
      List<UpnpDevice> dl;
      lock (devices) {
        if (!devices.TryGetValue(UUID, out dl)) {
          return;
        }
        devices.Remove(UUID);
      }
      foreach (var d in dl) {
        NotifyDevice(d, "byebye", true);
      }
      _logger.DebugFormat("Unregistered mount {0}", UUID);
    }

    public void Dispose()
    {
      _logger.Debug("Disposing SSDP");
      running = false;
      while (messageQueue.Count != 0) {
        datagramPosted.WaitOne();
      }

      client.DropMulticastGroup(SSDP_IP);

      notificationTimer.Enabled = false;
      queueTimer.Enabled = false;
      notificationTimer.Dispose();
      queueTimer.Dispose();
      datagramPosted.Dispose();
    }
  }
}
