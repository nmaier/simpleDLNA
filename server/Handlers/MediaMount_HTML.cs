using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal partial class MediaMount
  {
    private readonly List<string> htmlItemProperties = new List<string>() {
      "Type",
      "Duration",
      "Resolution",
      "Director",
      "Actors",
      "Performer",
      "Album",
      "Genre",
      "Date",
      "Size" };


    private IResponse ProcessHtmlRequest(IMediaItem aItem)
    {
      var item = aItem as IMediaFolder;
      if (item == null) {
        throw new Http404Exception();
      }
      var doc = new XmlDocument();
      doc.AppendChild(doc.CreateDocumentType("html", null, null, null));

      doc.AppendChild(doc.EL("html"));

      var head = doc.EL("head");
      doc.DocumentElement.AppendChild(head);
      head.AppendChild(doc.EL("title", text: string.Format("{0} — simple DLNA", item.Title)));
      head.AppendChild(doc.EL(
        "link",
        new AttributeCollection() { { "rel", "stylesheet" }, { "type", "text/css" }, { "href", prefix + "browse.css" } }
        ));

      var body = doc.EL("body");
      doc.DocumentElement.AppendChild(body);
      var article = doc.EL("article");
      body.AppendChild(article);

      article.AppendChild(doc.EL("h1", text: string.Format("Folder: {0}", item.Title)));

      XmlNode e;
      var folders = doc.EL("ul", new AttributeCollection() { { "class", "folders" } });
      if (item.Parent != null) {
        folders.AppendChild(e = doc.EL("li"));
        e.AppendChild(doc.EL(
            "a",
            new AttributeCollection() { { "href", String.Format("{0}index/{1}", prefix, item.Parent.Id) }, { "class", "parent" } },
            "Parent"
            ));
      }
      foreach (var i in item.ChildFolders) {
        folders.AppendChild(e = doc.EL("li"));
        e.AppendChild(doc.EL(
          "a",
          new AttributeCollection() { { "href", String.Format("{0}index/{1}", prefix, i.Id) } },
          string.Format("{0} ({1})", i.Title, i.ChildCount)
          ));
      }
      article.AppendChild(folders);

      var items = (XmlNode)null;
      article.AppendChild(items = doc.EL("ul", new AttributeCollection() { { "class", "items" } }));
      foreach (var i in item.ChildItems) {
        items.AppendChild(e = doc.EL("li"));
        var link = doc.EL(
          "a",
          new AttributeCollection() { { "href", string.Format("{0}file/{1}/{2}.{3}", prefix, i.Id, i.Title, DlnaMaps.Dlna2Ext[i.Type][0]) } }
          );
        var details = doc.EL("section");
        link.AppendChild(details);
        e.AppendChild(link);

        details.AppendChild(doc.EL("h3", new AttributeCollection { { "title", i.Title } }, i.Title));

        var props = i.Properties;
        if (props.ContainsKey("HasCover")) {
          details.AppendChild(doc.EL(
            "img",
            new AttributeCollection { { "title", "Cover image" }, { "alt", "Cover image" }, { "src", String.Format("{0}cover/{1}", prefix, i.Id) } }
            ));
        }

        var table = doc.EL("table");
        foreach (var p in htmlItemProperties) {
          string v;
          if (props.TryGetValue(p, out v)) {
            table.AppendChild(e = doc.EL("tr"));
            e.AppendChild(doc.EL("th", text: p));
            e.AppendChild(doc.EL("td", text: v));
          }
        }
        if (table.ChildNodes.Count != 0) {
          details.AppendChild(table);
        }

        string description;
        if (props.TryGetValue("Description", out description)) {
          link.AppendChild(doc.EL("p", new AttributeCollection() { { "class", "desc" } }, description));
        }
      }
      article.AppendChild(doc.EL("div", new AttributeCollection() { { "class", "clear" } }, string.Empty));

      var footer = doc.EL("footer");
      footer.AppendChild(doc.EL("img", new AttributeCollection() { { "src", "/icon/smallPNG" } }));
      footer.AppendChild(doc.EL("h3", text: string.Format(
        "simple DLNA Media Server: sdlna/{0}.{1}",
        Assembly.GetExecutingAssembly().GetName().Version.Major,
        Assembly.GetExecutingAssembly().GetName().Version.Minor
        )));
      footer.AppendChild(doc.EL("p", new AttributeCollection() { { "class", "desc" } }, "A simple, zero-config DLNA media server, that you can just fire up and be done with it."));
      footer.AppendChild(doc.EL("a", new AttributeCollection() { { "href", "https://github.com/nmaier/simpleDLNA/" } }, "Fork me on GitHub"));
      body.AppendChild(footer);

      return new StringResponse(HttpCodes.OK, doc.OuterXml);
    }
  }
}
