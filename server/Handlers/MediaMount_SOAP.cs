using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal partial class MediaMount
  {

    private static readonly string featureList = Encoding.UTF8.GetString(Properties.Resources.ResourceManager.GetObject("x_featurelist") as byte[]);
    private const string NS_CD = "urn:schemas-upnp-org:service:ContentDirectory:1";
    private const string NS_DC = "http://purl.org/dc/elements/1.1/";
    private const string NS_DIDL = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";
    private const string NS_DLNA = "urn:schemas-dlna-org:metadata-1-0/";
    private const string NS_SEC = "http://www.sec.co.kr/";
    private const string NS_SOAPENV = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string NS_UPNP = "urn:schemas-upnp-org:metadata-1-0/upnp/";
    private static IDictionary<string, AttributeCollection> SoapCache = new LruDictionary<string, AttributeCollection>(200);




    private void Browse_AddFolder(XmlDocument result, IMediaFolder f)
    {
      var meta = f as IMetaInfo;
      var container = result.CreateElement("", "container", NS_DIDL);
      container.SetAttribute("restricted", "0");
      container.SetAttribute("childCount", f.ChildCount.ToString());
      container.SetAttribute("id", f.Id);
      var parent = f.Parent;
      if (parent == null) {
        container.SetAttribute("parentID", "0");
      }
      else {
        container.SetAttribute("parentID", parent.Id);
      }

      var title = result.CreateElement("dc", "title", NS_DC);
      title.InnerText = f.Title;
      container.AppendChild(title);
      if (meta != null && meta.InfoDate != null) {
        var date = result.CreateElement("dc", "date", NS_DC);
        date.InnerText = meta.InfoDate.ToString("o");
        container.AppendChild(date);
      }

      var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
      objectClass.InnerText = "object.container";
      container.AppendChild(objectClass);
      result.DocumentElement.AppendChild(container);
    }

    private void Browse_AddItem(IRequest request, XmlDocument result, IMediaResource r)
    {
      var props = r.Properties;
      string prop = null;

      var item = result.CreateElement("", "item", NS_DIDL);
      item.SetAttribute("restricted", "1");
      item.SetAttribute("id", r.Id);
      item.SetAttribute("parentID", "0");

      var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
      switch (r.MediaType) {
        case MediaTypes.VIDEO:
          objectClass.InnerText = "object.item.videoItem.movie";
          break;
        case MediaTypes.IMAGE:
          objectClass.InnerText = "object.item.imageItem.photo";
          break;
        case MediaTypes.AUDIO:
          objectClass.InnerText = "object.item.audioItem.musicTrack";
          break;
        default:
          throw new NotSupportedException();
      }
      item.AppendChild(objectClass);

      var bookmarkable = r as IBookmarkable;
      if (bookmarkable != null) {
        var bookmark = bookmarkable.Bookmark;
        if (bookmark.HasValue) {
          var dcmInfo = result.CreateElement("sec", "dcmInfo", NS_SEC);
          dcmInfo.InnerText = string.Format("BM={0}", bookmark.Value);
          item.AppendChild(dcmInfo);
        }
      }

      if (props.TryGetValue("DateO", out prop)) {
        var e = result.CreateElement("dc", "date", NS_DC);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Genre", out prop)) {
        var e = result.CreateElement("upnp", "genre", NS_UPNP);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Description", out prop)) {
        var e = result.CreateElement("dc", "description", NS_DC);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Artist", out prop)) {
        var e = result.CreateElement("upnp", "artist", NS_UPNP);
        e.SetAttribute("role", "AlbumArtist");
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Performer", out prop)) {
        var e = result.CreateElement("upnp", "artist", NS_UPNP);
        e.SetAttribute("role", "Performer");
        e.InnerText = prop;
        item.AppendChild(e);
        e = result.CreateElement("dc", "creator", NS_DC);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Album", out prop)) {
        var e = result.CreateElement("upnp", "album", NS_UPNP);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Track", out prop)) {
        var e = result.CreateElement("upnp", "originalTrackNumber", NS_UPNP);
        e.InnerText = prop;
        item.AppendChild(e);
      }
      if (props.TryGetValue("Creator", out prop)) {
        var e = result.CreateElement("dc", "creator", NS_DC);
        e.InnerText = prop;
        item.AppendChild(e);
      }

      if (props.TryGetValue("Director", out prop)) {
        var e = result.CreateElement("upnp", "director", NS_UPNP);
        e.InnerText = prop;
        item.AppendChild(e);
      }

      var mvi = r as IMetaVideoItem;
      if (mvi != null) {
        try {
          var actors = mvi.MetaActors;
          if (actors != null) {
            foreach (var actor in actors) {
              var e = result.CreateElement("upnp", "actor", NS_UPNP);
              e.InnerText = actor;
              item.AppendChild(e);
            }
          }
        }
        catch (Exception) { }
      }

      var title = result.CreateElement("dc", "title", NS_DC);
      title.InnerText = r.Title;
      item.AppendChild(title);

      var res = result.CreateElement("", "res", NS_DIDL);
      res.InnerText = String.Format(
        "http://{0}:{1}{2}file/{3}",
        request.LocalEndPoint.Address,
        request.LocalEndPoint.Port,
        prefix,
        r.Id
        );

      if (props.TryGetValue("SizeRaw", out prop)) {
        res.SetAttribute("size", prop);
      }
      if (props.TryGetValue("Resolution", out prop)) {
        res.SetAttribute("resolution", prop);
      }
      if (props.TryGetValue("Duration", out prop)) {
        res.SetAttribute("duration", prop);
      }

      var pn = r.PN;
      var mime = DlnaMaps.Mime[r.Type];
      res.SetAttribute("protocolInfo", String.Format(
          "http-get:*:{1}:{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=01500000000000000000000000000000",
          pn, mime
          ));
      item.AppendChild(res);

      var cover = r as IMediaCover;
      if (cover != null) {
        try {
          var c = cover.Cover;
          var curl = String.Format(
            "http://{0}:{1}{2}cover/{3}",
            request.LocalEndPoint.Address,
            request.LocalEndPoint.Port,
            prefix,
            r.Id
            );
          var icon = result.CreateElement("upnp", "albumArtURI", NS_UPNP);
          var profile = result.CreateAttribute("dlna", "profileID", NS_DLNA);
          profile.InnerText = "JPEG_TN";
          icon.SetAttributeNode(profile);
          icon.InnerText = curl;
          item.AppendChild(icon);
          icon = result.CreateElement("upnp", "icon", NS_UPNP);
          profile = result.CreateAttribute("dlna", "profileID", NS_DLNA);
          profile.InnerText = "JPEG_TN";
          icon.SetAttributeNode(profile);
          icon.InnerText = curl;
          item.AppendChild(icon);

          res = result.CreateElement("", "res", NS_DIDL);
          res.InnerText = curl;

          pn = c.PN;
          mime = DlnaMaps.Mime[DlnaType.JPEG];
          var width = c.MetaWidth;
          var height = c.MetaHeight;
          if (width.HasValue && height.HasValue) {
            res.SetAttribute("resolution", string.Format("{0}x{1}", width.Value, height.Value));
          }
          else {
            res.SetAttribute("resolution", "200x200");
          }
          res.SetAttribute("protocolInfo", "http-get:*:image/jpeg:DLNA.ORG_PN=JPEG_TN;DLNA.ORG_OP=00;DLNA.ORG_CI=1;DLNA.ORG_FLAGS=00D00000000000000000000000000000");
          item.AppendChild(res);
        }
        catch (Exception) { }
      }
      result.DocumentElement.AppendChild(item);
    }

    private IEnumerable<KeyValuePair<string, string>> HandleBrowse(IRequest request, IHeaders sparams)
    {
      var key = Prefix + sparams.HeaderBlock;
      AttributeCollection rv;
      if (SoapCache.TryGetValue(key, out rv)) {
        return rv;
      }

      string id = sparams["ObjectID"];
      string flag = sparams["BrowseFlag"];

      int requested = 20;
      int provided = 0;
      int start = 0;
      try {
        if (int.TryParse(sparams["RequestedCount"], out requested) && requested <= 0) {
          requested = 20;
        }
        if (int.TryParse(sparams["StartingIndex"], out start) && start <= 0) {
          start = 0;
        }
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
      didl.SetAttribute("xmlns:sec", NS_SEC);
      result.AppendChild(didl);

      if (flag == "BrowseMetadata") {
        Browse_AddFolder(result, root);
        provided++;
      }
      else {
        foreach (var i in root.ChildFolders) {
          if (start > 0) {
            start--;
            continue;
          }
          Browse_AddFolder(result, i);
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
            Browse_AddItem(request, result, i);
            if (++provided == requested) {
              break;
            }
          }
        }
      }
      var resXML = result.OuterXml;
      rv = new AttributeCollection() {
        {"Result", resXML},
        {"NumberReturned", provided.ToString()},
        {"TotalMatches", root.ChildCount.ToString()},
        {"UpdateID", systemID.ToString()}
      };
      SoapCache[key] = rv;
      return rv;
    }

    private static IHeaders HandleGetSearchCapabilities()
    {
      return new RawHeaders() { { "SearchCaps", "" } };
    }

    private static IHeaders HandleGetSortCapabilities()
    {
      return new RawHeaders() { { "SortCaps", "" } };
    }

    private IHeaders HandleGetSystemUpdateID()
    {
      return new RawHeaders() { { "Id", systemID.ToString() } };
    }

    private static IHeaders HandleXGetFeatureList()
    {
      return new RawHeaders() { { "FeatureList", featureList } };
    }

    private IHeaders HandleXSetBookmark(IHeaders sparams)
    {
      var id = sparams["ObjectID"];
      var item = GetItem(id) as IBookmarkable;
      if (item != null) {
        ulong newbookmark = ulong.Parse(sparams["PosSecond"]);
        if (newbookmark > 30) {
          // rewind 5 seconds
          newbookmark -= 5;
        }
        if (newbookmark > 30 || !item.Bookmark.HasValue || item.Bookmark.Value < 60) {
          item.Bookmark = newbookmark;
          lock (SoapCache) {
            SoapCache.Clear();
          }
        }
      }
      return new RawHeaders();
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
            result = HandleGetSearchCapabilities();
            break;
          case "GetSortCapabilities":
            result = HandleGetSortCapabilities();
            break;
          case "GetSystemUpdateID":
            result = HandleGetSystemUpdateID();
            break;
          case "Browse":
            result = HandleBrowse(request, sparams);
            break;
          case "X_GetFeatureList":
            result = HandleXGetFeatureList();
            break;
          case "X_SetBookmark":
            result = HandleXSetBookmark(sparams);
            break;
          default:
            throw new Http404Exception();
        }
        var response = env.CreateElement(String.Format("u:{0}Response", method.LocalName), method.NamespaceURI);
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
        WarnFormat("Invalid call: Action: {0}, Params: {1}, Problem {2}", method.LocalName, sparams, ex.Message);
      }

      var rv = new StringResponse(code, "text/xml", env.OuterXml);
      rv.Headers.Add("EXT", "");
      return rv;
    }
  }
}
