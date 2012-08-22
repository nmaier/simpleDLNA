using System;
using System.Collections.Generic;
using System.Xml;
using NMaier.sdlna.Server.Metadata;
using NMaier.sdlna.Util;

namespace NMaier.sdlna.Server
{
  internal partial class MediaMount
  {

    private const string NS_CD = "urn:schemas-upnp-org:service:ContentDirectory:1";
    private const string NS_DC = "http://purl.org/dc/elements/1.1/";
    private const string NS_DIDL = "urn:schemas-upnp-org:metadata-1-0/DIDL-Lite/";
    private const string NS_DLNA = "urn:schemas-dlna-org:metadata-1-0/";
    private const string NS_SOAPENV = "http://schemas.xmlsoap.org/soap/envelope/";
    private const string NS_UPNP = "urn:schemas-upnp-org:metadata-1-0/upnp/";


    private void Browse_AddFolder(XmlDocument result, IMediaFolder f)
    {
      var meta = f as IMetaInfo;
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
      if (meta != null && meta.Date != null) {
        var date = result.CreateElement("dc", "date", NS_DC);
        date.InnerText = meta.Date.ToString("o");
        container.AppendChild(date);
      }

      var objectClass = result.CreateElement("upnp", "class", NS_UPNP);
      objectClass.InnerText = "object.container";
      container.AppendChild(objectClass);
      result.DocumentElement.AppendChild(container);
    }

    private void Browse_AddItem(IRequest request, XmlDocument result, IMediaResource r)
    {
      var item = result.CreateElement("", "item", NS_DIDL);
      item.SetAttribute("restricted", "1");
      item.SetAttribute("id", r.ID);
      var parent = r.Parent;
      if (parent == null) {
        parent = Root;
      }
      item.SetAttribute("parentID", parent.ID);

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
      var meta = r as IMetaInfo;
      if (meta != null && meta.Date != null) {
        try {
          var date = result.CreateElement("dc", "date", NS_DC);
          date.InnerText = meta.Date.ToString("o");
          item.AppendChild(date);
        }
        catch (Exception) { }
      }
      if (r is IMetaGenre) {
        try {
          var genre = (r as IMetaGenre).MetaGenre;
          if (genre != null) {
            var e = result.CreateElement("upnp", "genre", NS_UPNP);
            e.InnerText = genre;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }
      }
      if (r is IMetaDescription) {
        try {
          var desc = (r as IMetaDescription).MetaDescription;
          if (desc != null) {
            var e = result.CreateElement("dc", "description", NS_DC);
            e.InnerText = desc;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }
      }
      if (r is IMetaAudioItem) {
        var mai = r as IMetaAudioItem;
        try {
          var artist = mai.MetaArtist;
          if (artist != null) {
            var e = result.CreateElement("upnp", "artist", NS_UPNP);
            e.SetAttribute("role", "AlbumArtist");
            e.InnerText = artist;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }
        try {
          var performer = mai.MetaPerformer;
          if (performer != null) {
            var e = result.CreateElement("upnp", "artist", NS_UPNP);
            e.SetAttribute("role", "Performer");
            e.InnerText = performer;
            item.AppendChild(e);
            e = result.CreateElement("dc", "creator", NS_DC);
            e.InnerText = performer;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }

        try {
          if (mai.MetaAlbum != null) {
            var e = result.CreateElement("upnp", "album", NS_UPNP);
            e.InnerText = mai.MetaAlbum;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }

        try {
          if (mai.MetaTrack != null) {
            var e = result.CreateElement("upnp", "originalTrackNumber", NS_UPNP);
            e.InnerText = mai.MetaTrack.Value.ToString();
            item.AppendChild(e);
          }
        }
        catch (Exception) { }
      }

      if (r is IMetaImageItem) {
        try {
          var creator = (r as IMetaImageItem).MetaCreator;
          if (creator != null) {
            var e = result.CreateElement("dc", "creator", NS_DC);
            e.InnerText = creator;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }
      }

      if (r is IMetaVideoItem) {
        var mvi = r as IMetaVideoItem;
        try {
          var director = mvi.MetaDirector;
          if (director != null) {
            var e = result.CreateElement("upnp", "director", NS_UPNP);
            e.InnerText = director;
            item.AppendChild(e);
          }
        }
        catch (Exception) { }

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
        r.ID
        );

      if (meta != null) {
        try {
          var size = meta.Size;
          if (size.HasValue) {
            res.SetAttribute("size", size.Value.ToString());
          }
        }
        catch (Exception) { }
      }
      if (r is IMetaResolution) {
        try {
          var metaRes = r as IMetaResolution;
          res.SetAttribute("resolution", String.Format("{0}x{1}", metaRes.MetaWidth, metaRes.MetaHeight));
        }
        catch (Exception) { }
      }
      if (r is IMetaDuration) {
        try {
          var duration = (r as IMetaDuration).MetaDuration;
          if (duration.HasValue) {
            res.SetAttribute("duration", duration.Value.ToString("g"));
          }
        }
        catch (Exception) { }
      }


      var pn = r.PN;
      var mime = DlnaMaps.Mime[r.Type];
      res.SetAttribute("protocolInfo", String.Format(
          "http-get:*:{1}:{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=01500000000000000000000000000000",
          pn, mime
          ));
      item.AppendChild(res);

      if (r is IMediaCover) {
        try {
          var c = (r as IMediaCover).Cover;
          var curl = String.Format(
            "http://{0}:{1}{2}cover/{3}",
            request.LocalEndPoint.Address,
            request.LocalEndPoint.Port,
            prefix,
            r.ID
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
          mime = DlnaMaps.Mime[DlnaTypes.JPEG];
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

    private static void Browse_AddResponseParam(XmlDocumentFragment response, string name, string value)
    {
      var node = response.OwnerDocument.CreateElement("", name, NS_SOAPENV);
      node.InnerText = value;
      response.AppendChild(node);
    }

    private IEnumerable<KeyValuePair<string, string>> HandleBrowse(IRequest request, IHeaders sparams)
    {
      string id = sparams["ObjectID"];
      string flag = sparams["BrowseFlag"];
      if (id == "0") {
        id = Root.ID;
      }
      Debug(id);
      Debug(flag);
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
      Debug(resXML);
      return new ResList() {
        {"Result", resXML },
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
            result = HandleBrowse(request, sparams);
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
