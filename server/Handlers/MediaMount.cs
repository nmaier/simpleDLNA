using System;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Collections.Generic;

namespace NMaier.sdlna.Server
{
  internal class MediaMount : Logging, IMediaServer, IPrefixHandler
  {

    private readonly string baseURI;
    private readonly string descriptor;
    private const string NS_CD = "urn:schemas-upnp-org:service:ContentDirectory:1";
    private const string NS_DC = "http://purl.org/dc/elements/1.1/";
    private const string NS_DIDL = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";
    private const string NS_DLNA = "urn:schemas-dlna-org:metadata-1-0/";
    private const string NS_SOAPENV = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string NS_UPNP = "urn:schemas-upnp-org:metadata-1-0/upnp/";
    private readonly string prefix;
    private readonly IMediaServer server;
    private uint systemID = 1;



    public MediaMount(IMediaServer aServer, string aBaseURI)
    {
      server = aServer;
      baseURI = aBaseURI;
      prefix = String.Format("/{0}/", server.UUID);
      descriptor = GenerateDescriptor();
      if (server is IVolatileMediaServer) {
        (server as IVolatileMediaServer).Changed += ChangedServer;
      }
    }



    public string DescriptorURI
    {
      get { return String.Format("{0}description.xml", prefix); }
    }

    public string FriendlyName
    {
      get { return server.FriendlyName; }
    }

    public string Prefix
    {
      get { return prefix; }
    }

    public IMediaFolder Root
    {
      get { return server.Root; }
    }

    public Guid UUID
    {
      get { return server.UUID; }
    }




    public IMediaItem GetItem(string id)
    {
      return server.GetItem(id);
    }

    public IResponse HandleRequest(IRequest request)
    {
      var path = request.Path.Substring(prefix.Length);
      Debug(path);
      if (path == "description.xml") {
        return new StringResponse(HttpCodes.OK, "text/xml", descriptor);
      }
      if (path == "contentDirectory.xml") {
        return new ResourceResponse(HttpCodes.OK, "text/xml", "contentdirectory");
      }
      if (path == "control") {
        return ProcessSoapRequest(request);
      }
      if (path.StartsWith("file/")) {
        var id = path.Substring("file/".Length);
        var item = GetItem(id) as IMediaResource;
        return new ItemResponse(request, item);
      }
      if (path.StartsWith("cover/")) {
        var id = path.Substring("cover/".Length);
        var item = GetItem(id) as IMediaCover;
        return new ItemResponse(request, item.Cover, "Interactive");
      }
      WarnFormat("Did not understand {0} {1}", request.Method, path);
      throw new Http404Exception();
    }

    private void AddFolder(XmlDocument result, IMediaFolder f)
    {
      var meta = f as IMediaItemMetaData;
      var container = result.CreateElement("", "container", NS_DIDL);
      container.SetAttribute("restricted", "0");
      container.SetAttribute("childCount", f.ChildCount.ToString());
      container.SetAttribute("id", f.ID);
      var parent = f.Parent;
      if (parent == null) {
        container.SetAttribute("parentID", Root.ID);
      }
      else {
        container.SetAttribute("parentID", parent.ID);
      }

      var title = result.CreateElement("dc", "title", NS_DC);
      title.InnerText = f.Title;
      container.AppendChild(title);
      if (meta != null) {
        var date = result.CreateElement("dc", "date", NS_DC);
        date.InnerText = meta.ItemDate.ToString("o");
        container.AppendChild(date);
      }

      var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
      objectClass.InnerText = "object.container";
      container.AppendChild(objectClass);
      result.DocumentElement.AppendChild(container);
    }

    private void AddItem(XmlDocument result, IMediaResource r)
    {
      var meta = r as IMediaItemMetaData;
      var item = result.CreateElement("", "item", NS_DIDL);
      item.SetAttribute("restricted", "1");
      item.SetAttribute("id", r.ID);
      var parent = r.Parent;
      if (parent == null) {
        parent = Root;
      }
      item.SetAttribute("parentID", parent.ID);

      var title = result.CreateElement("dc", "title", NS_DC);
      title.InnerText = r.Title;
      item.AppendChild(title);
      if (meta != null) {
        var date = result.CreateElement("dc", "date", NS_DC);
        date.InnerText = meta.ItemDate.ToString("o");
        item.AppendChild(date);
      }

      var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
      if (r.Type == DlnaTypes.JPEG) {
        objectClass.InnerText = "object.item.imageItem.photo";
      }
      else {
        objectClass.InnerText = "object.item.videoItem";
      }
      item.AppendChild(objectClass);

      var res = result.CreateElement("", "res", NS_DIDL);
      res.InnerText = baseURI + prefix + "file/" + r.ID;

      if (meta != null) {
        res.SetAttribute("size", meta.ItemSize.ToString());
      }
      var pn = r.PN;
      var mime = DlnaMaps.Mime[r.Type];
      res.SetAttribute("protocolInfo", String.Format(
          "http-get:*:{1}:{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=01500000000000000000000000000000",
          pn, mime
          ));
      item.AppendChild(res);

      if (r is IMediaCover) {
        var c = (r as IMediaCover).Cover;
        if (c != null) {
          var curl = baseURI + prefix + "cover/" + r.ID;
          var icon = result.CreateElement("upnp", "albumArtURI", NS_UPNP);
          icon.SetAttribute("dlna:profileID", "JPEG_TN");
          icon.InnerText = curl;
          item.AppendChild(icon);
          icon = result.CreateElement("upnp", "icon", NS_UPNP);
          icon.SetAttribute("dlna:profileID", "JPEG_TN");
          icon.InnerText = curl;
          item.AppendChild(icon);

          res = result.CreateElement("", "res", NS_DIDL);
          res.InnerText = curl;

          pn = c.PN;
          mime = DlnaMaps.Mime[DlnaTypes.JPEG];
          res.SetAttribute("resolution", "160x120");
          res.SetAttribute("protocolInfo", "http-get:*:image/jpeg:DLNA.ORG_PN=JPEG_TN;DLNA.ORG_OP=00;DLNA.ORG_CI=1;DLNA.ORG_FLAGS=00D00000000000000000000000000000");
          item.AppendChild(res);
        }
      }
      result.DocumentElement.AppendChild(item);
    }

    private static void AddResponseParam(XmlDocumentFragment response, string name, string value)
    {
      var node = response.OwnerDocument.CreateElement("", name, NS_SOAPENV);
      node.InnerText = value;
      response.AppendChild(node);
    }

    private void ChangedServer(object sender, EventArgs e)
    {
      InfoFormat("Rescanned mount {0}", UUID);
      systemID++;
    }

    private string GenerateDescriptor()
    {
      var doc = new XmlDocument();
      doc.LoadXml(Encoding.UTF8.GetString(Properties.Resources.description));
      doc.GetElementsByTagName("UDN").Item(0).InnerText = String.Format("uuid:{0}", UUID);
      doc.GetElementsByTagName("modelNumber").Item(0).InnerText = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      doc.GetElementsByTagName("friendlyName").Item(0).InnerText = "sdlna — " + FriendlyName;
      doc.GetElementsByTagName("SCPDURL").Item(0).InnerText = String.Format("{0}contentDirectory.xml", prefix);
      doc.GetElementsByTagName("controlURL").Item(0).InnerText = String.Format("{0}control", prefix);
      doc.GetElementsByTagName("eventSubURL").Item(0).InnerText = String.Format("{0}events", prefix);

      return doc.OuterXml;
    }

    private IEnumerable<KeyValuePair<string, string>> HandleBrowse(IHeaders sparams)
    {
      string id = sparams["ObjectID"];
      string flag = sparams["BrowseFlag"];
      if (id == "0") {
        id = Root.ID;
      }
      Debug(id);
      Debug(flag);
      uint requested = 20;
      uint provided = 0, start = 0;
      try {
        uint.TryParse(sparams["RequestedCount"], out requested);
        uint.TryParse(sparams["StartingIndex"], out start);
      }
      catch (Exception ex) {
        Debug("Not all params provided", ex);
      }

      var root = GetItem(id) as IMediaFolder;
      var result = new XmlDocument();

      var didl = result.CreateElement("", "DIDL-Lite", NS_DIDL);
      didl.SetAttribute("xmlns:dc", NS_DC);
      didl.SetAttribute("xmlns:dlna", NS_DLNA);
      didl.SetAttribute("xmlns:upnp", NS_UPNP);
      result.AppendChild(didl);

      if (flag == "BrowseMetadata") {
        AddFolder(result, root);
        provided++;
      }
      else {
        foreach (var i in root.ChildFolders) {
          if (start > 0) {
            start--;
            continue;
          }
          AddFolder(result, i);
          if (++provided == requested) {
            break;
          }
        }
        if (provided != requested) {
          foreach (var i in root.ChildItems) {
            if (start > 0) {
              start--;
              continue;
            }
            AddItem(result, i);
            if (++provided == requested) {
              break;
            }
          }
        }
      }

      return new ResList() {
        {"Result", result.OuterXml },
        {"NumberReturned", provided.ToString() },
        {"TotalMatches", root.ChildCount.ToString() },
        {"UpdateID", systemID.ToString() }
      };
    }

    private IHeaders HandleGetSearchCapabilities(IHeaders sparams)
    {
      return new RawHeaders() { { "SearchCaps", "" } };
    }

    private IHeaders HandleGetSortCapabilities(IHeaders sparams)
    {
      return new RawHeaders() { { "SortCaps", "" } };
    }

    private IHeaders HandleGetSystemUpdateID(IHeaders sparams)
    {
      return new RawHeaders() { { "Id", systemID.ToString() } };
    }

    private IResponse ProcessSoapRequest(IRequest request)
    {
      var soap = new XmlDocument();
      soap.LoadXml(request.Body);
      var sparams = new RawHeaders();
      var body = soap.GetElementsByTagName("Body", NS_SOAPENV).Item(0);
      var method = body.FirstChild;
      foreach (var p in method.ChildNodes) {
        var e = p as XmlElement;
        if (e == null) {
          continue;
        }
        sparams.Add(e.LocalName, e.InnerText.Trim());
      }
      var env = new XmlDocument();
      env.AppendChild(env.CreateXmlDeclaration("1.0", "utf-8", "yes"));
      var envelope = env.CreateElement("SOAP-ENV", "Envelope", NS_SOAPENV);
      env.AppendChild(envelope);
      envelope.SetAttribute("encodingStyle", NS_SOAPENV, "http://schemas.xmlsoap.org/soap/encoding/");

      var rbody = env.CreateElement("SOAP-ENV:Body", NS_SOAPENV);
      env.DocumentElement.AppendChild(rbody);

      var code = HttpCodes.OK;
      try {
        IEnumerable<KeyValuePair<string, string>> result;
        switch (method.LocalName) {
          case "GetSearchCapabilities":
            result = HandleGetSearchCapabilities(sparams);
            break;
          case "GetSortCapabilities":
            result = HandleGetSortCapabilities(sparams);
            break;
          case "GetSystemUpdateID":
            result = HandleGetSystemUpdateID(sparams);
            break;
          case "Browse":
            result = HandleBrowse(sparams);
            break;
          default:
            throw new Http404Exception();
        }
        var response = env.CreateElement(String.Format("u:{0}Response", method.LocalName), NS_CD);
        rbody.AppendChild(response);

        foreach (var i in result) {
          var ri = env.CreateElement(i.Key);
          ri.InnerText = i.Value;
          response.AppendChild(ri);
        }
      }
      catch (Exception ex) {
        code = HttpCodes.INTERNAL_ERROR;
        var fault = env.CreateElement("SOAP-ENV", "Fault", NS_SOAPENV);
        var faultCode = env.CreateElement("faultcode");
        faultCode.InnerText = "500";
        fault.AppendChild(faultCode);
        var faultString = env.CreateElement("faultstring");
        faultString.InnerText = ex.ToString();
        fault.AppendChild(faultString);
        var detail = env.CreateDocumentFragment();
        detail.InnerXml = "<detail><UPnPError xmlns=\"urn:schemas-upnp-org:control-1-0\"><errorCode>401</errorCode><errorDescription>Invalid Action</errorDescription></UPnPError></detail>";
        fault.AppendChild(detail);

        rbody.AppendChild(fault);
      }

      var rv = new StringResponse(code, "text/xml", env.OuterXml);
      rv.Headers.Add("EXT", "");
      return rv;
    }
  }
}
