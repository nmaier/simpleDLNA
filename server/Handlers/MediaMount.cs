using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Reflection;
using System.Xml;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Server.Properties;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed partial class MediaMount
    : Logging, IMediaServer, IPrefixHandler
  {
    private static uint mount;

    private readonly Dictionary<IPAddress, Guid> guidsForAddresses =
      new Dictionary<IPAddress, Guid>();

    private readonly IMediaServer server;

    private uint systemID = 1;

    public MediaMount(IMediaServer aServer)
    {
      server = aServer;
      Prefix = $"/mm-{++mount}/";
      var vms = server as IVolatileMediaServer;
      if (vms != null) {
        vms.Changed += ChangedServer;
      }
    }

    public string DescriptorURI => $"{Prefix}description.xml";

    public IHttpAuthorizationMethod Authorizer => server.Authorizer;

    public string FriendlyName => server.FriendlyName;

    public Guid UUID => server.UUID;

    public IMediaItem GetItem(string id)
    {
      return server.GetItem(id);
    }

    public string Prefix { get; }

    public IResponse HandleRequest(IRequest request)
    {
      if (Authorizer != null &&
          !IPAddress.IsLoopback(request.RemoteEndpoint.Address) &&
          !Authorizer.Authorize(
            request.Headers,
            request.RemoteEndpoint,
            IP.GetMAC(request.RemoteEndpoint.Address)
            )) {
        throw new HttpStatusException(HttpCode.Denied);
      }

      var path = request.Path.Substring(Prefix.Length);
      Debug(path);
      if (path == "description.xml") {
        return new StringResponse(
          HttpCode.Ok,
          "text/xml",
          GenerateDescriptor(request.LocalEndPoint.Address)
          );
      }
      if (path == "contentDirectory.xml") {
        return new ResourceResponse(
          HttpCode.Ok,
          "text/xml",
          "contentdirectory"
          );
      }
      if (path == "connectionManager.xml") {
        return new ResourceResponse(
          HttpCode.Ok,
          "text/xml",
          "connectionmanager"
          );
      }
      if (path == "MSMediaReceiverRegistrar.xml") {
        return new ResourceResponse(
          HttpCode.Ok,
          "text/xml",
          "MSMediaReceiverRegistrar"
          );
      }
      if (path == "control") {
        return ProcessSoapRequest(request);
      }
      if (path.StartsWith("file/", StringComparison.Ordinal)) {
        var id = path.Split('/')[1];
        InfoFormat("Serving file {0}", id);
        var item = GetItem(id) as IMediaResource;
        return new ItemResponse(Prefix, request, item);
      }
      if (path.StartsWith("cover/", StringComparison.Ordinal)) {
        var id = path.Split('/')[1];
        InfoFormat("Serving cover {0}", id);
        var item = GetItem(id) as IMediaCover;
        if (item == null) {
          throw new HttpStatusException(HttpCode.NotFound);
        }
        return new ItemResponse(Prefix, request, item.Cover, "Interactive");
      }
      if (path.StartsWith("subtitle/", StringComparison.Ordinal)) {
        var id = path.Split('/')[1];
        InfoFormat("Serving subtitle {0}", id);
        var item = GetItem(id) as IMetaVideoItem;
        if (item == null) {
          throw new HttpStatusException(HttpCode.NotFound);
        }
        return new ItemResponse(Prefix, request, item.Subtitle, "Background");
      }

      if (string.IsNullOrEmpty(path) || path == "index.html") {
        return new Redirect(request, Prefix + "index/0");
      }
      if (path.StartsWith("index/", StringComparison.Ordinal)) {
        var id = path.Substring("index/".Length);
        var item = GetItem(id);
        return ProcessHtmlRequest(item);
      }
      if (request.Method == "SUBSCRIBE") {
        var res = new StringResponse(HttpCode.Ok, string.Empty);
        res.Headers.Add("SID", $"uuid:{Guid.NewGuid()}");
        res.Headers.Add("TIMEOUT", request.Headers["timeout"]);
        return res;
      }
      if (request.Method == "UNSUBSCRIBE") {
        return new StringResponse(HttpCode.Ok, string.Empty);
      }
      WarnFormat("Did not understand {0} {1}", request.Method, path);
      throw new HttpStatusException(HttpCode.NotFound);
    }

    private void ChangedServer(object sender, EventArgs e)
    {
      soapCache.Clear();
      InfoFormat("Rescanned mount {0}", UUID);
      systemID++;
    }

    [SuppressMessage("ReSharper", "PossibleNullReferenceException")]
    private string GenerateDescriptor(IPAddress source)
    {
      var doc = new XmlDocument();
      doc.LoadXml(Resources.description);
      Guid guid;
      guidsForAddresses.TryGetValue(source, out guid);
      doc.SelectSingleNode("//*[local-name() = 'UDN']").InnerText =
        $"uuid:{guid}";
      doc.SelectSingleNode("//*[local-name() = 'modelNumber']").InnerText =
        Assembly.GetExecutingAssembly().GetName().Version.ToString();
      doc.SelectSingleNode("//*[local-name() = 'friendlyName']").InnerText =
        FriendlyName + " — sdlna";

      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:ContentDirectory:1']/../*[local-name() = 'SCPDURL']").InnerText =
        $"{Prefix}contentDirectory.xml";
      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:ContentDirectory:1']/../*[local-name() = 'controlURL']").InnerText =
        $"{Prefix}control";
      doc.SelectSingleNode("//*[local-name() = 'eventSubURL']").InnerText =
        $"{Prefix}events";

      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'SCPDURL']").InnerText =
        $"{Prefix}connectionManager.xml";
      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'controlURL']").InnerText
        =
        $"{Prefix}control";
      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:ConnectionManager:1']/../*[local-name() = 'eventSubURL']").InnerText
        =
        $"{Prefix}events";

      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'SCPDURL']")
        .InnerText =
        $"{Prefix}MSMediaReceiverRegistrar.xml";
      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'controlURL']")
        .InnerText =
        $"{Prefix}control";
      doc.SelectSingleNode(
        "//*[text() = 'urn:schemas-upnp-org:service:X_MS_MediaReceiverRegistrar:1']/../*[local-name() = 'eventSubURL']")
        .InnerText =
        $"{Prefix}events";

      return doc.OuterXml;
    }

    public void AddDeviceGuid(Guid guid, IPAddress address)
    {
      guidsForAddresses.Add(address, guid);
    }
  }
}
