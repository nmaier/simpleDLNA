using System.Collections.Generic;
using System.Reflection;
using System.Xml;
using NMaier.sdlna.Util;

namespace NMaier.sdlna.Server
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
      "Size",
    };

    private IResponse ProcessHtmlRequest(IRequest request, IMediaItem aItem)
    {
      var item = aItem as IMediaFolder;
      if (item == null) {
        throw new Http404Exception();
      }
      var doc = new XmlDocument();
      doc.AppendChild(doc.CreateDocumentType("html", null, null, null));

      doc.AppendChild(doc.E("html"));

      var head = doc.E("head");
      doc.DocumentElement.AppendChild(head);
      head.AppendChild(doc.E("title", string.Format("{0} — simple DLNA", item.Title)));
      head.AppendChild(doc.E(
        "link",
        new ResList() { { "rel", "stylesheet" }, { "type", "text/css" }, { "href", prefix + "browse.css" } }
        ));

      var body = doc.E("body");
      doc.DocumentElement.AppendChild(body);
      var article = doc.E("article");
      body.AppendChild(article);

      article.AppendChild(doc.E("h1", string.Format("Folder: {0}", item.Title)));

      XmlElement e;
      var folders = doc.E("ul", new ResList() { { "class", "folders" } });
      if (item.Parent != null) {
        folders.AppendChild(e = doc.E("li"));
        e.AppendChild(doc.E(
            "a",
            new ResList() { { "href", prefix + "index/" + item.Parent.ID }, { "class", "parent" } },
            "Parent"
            ));
      }
      foreach (var i in item.ChildFolders) {
        folders.AppendChild(e = doc.E("li"));
        e.AppendChild(doc.E(
          "a",
          new ResList() { { "href", prefix + "index/" + i.ID } },
          string.Format("{0} ({1})", i.Title, i.ChildCount)
          ));
      }
      article.AppendChild(folders);

      XmlElement items = null;
      article.AppendChild(items = doc.E("ul", new ResList() { { "class", "items" } }));
      foreach (var i in item.ChildItems) {
        items.AppendChild(e = doc.E("li"));
        var link = doc.E(
          "a",
          new ResList() { { "href", string.Format("{0}file/{1}/{2}.{3}", prefix, i.ID, i.Title, DlnaMaps.Dlna2Ext[i.Type][0]) } }
          );
        var details = doc.E("section");
        link.AppendChild(details);
        e.AppendChild(link);

        details.AppendChild(doc.E("h3", new ResList { { "title", i.Title }}, i.Title));
        
        var props = i.Properties;
        if (props.ContainsKey("HasCover")) {
          details.AppendChild(doc.E(
            "img",
            new ResList { { "title", "Cover image" }, { "alt", "Cover image" }, { "src", prefix + "cover/" + i.ID } }
            ));
        }

        var table = doc.E("table");
        foreach (var p in htmlItemProperties) {
          string v;
          if (props.TryGetValue(p, out v)) {
            table.AppendChild(e = doc.E("tr"));
            e.AppendChild(doc.E("th", p));
            e.AppendChild(doc.E("td", v));
          }
        }
        if (table.ChildNodes.Count != 0) {
          details.AppendChild(table);
        }

        string description;
        if (props.TryGetValue("Description", out description)) {
          link.AppendChild(doc.E("p", new ResList() { { "class", "desc" } }, description));
        }
      }
      article.AppendChild(doc.E("div", new ResList() { { "class", "clear" } }, ""));

      var footer = doc.E("footer");
      footer.AppendChild(doc.E("img", new ResList() { { "src", "/icon/smallPNG" } }));
      footer.AppendChild(doc.E("h3", string.Format(
        "simple DLNA Media Server: sdlna/{0}.{1}",
        Assembly.GetExecutingAssembly().GetName().Version.Major,
        Assembly.GetExecutingAssembly().GetName().Version.Minor
        )));
      footer.AppendChild(doc.E("p", new ResList() { { "class", "desc" } }, "A simple, zero-config DLNA media server, that you can just fire up and be done with it."));
      footer.AppendChild(doc.E("a", new ResList() { { "href", "https://github.com/nmaier/sdlna/" } }, "Fork me on GitHub"));
      body.AppendChild(footer);

      return new StringResponse(HttpCodes.OK, doc.OuterXml);
    }
  }
}
