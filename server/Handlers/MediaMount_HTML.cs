using NMaier.SimpleDlna.Utilities;
using System;
using System.Collections.Generic;
using System.Xml;

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
        throw new HttpStatusException(HttpCode.NotFound);
      }

      var article = HtmlTools.CreateHtmlArticle(
        string.Format("Folder: {0}", item.Title));
      var document = article.OwnerDocument;

      XmlNode e;
      var folders = document.EL(
        "ul",
        new AttributeCollection() { { "class", "folders" } }
        );
      if (item.Parent != null) {
        folders.AppendChild(e = document.EL("li"));
        e.AppendChild(document.EL(
          "a",
          new AttributeCollection() {
            { "href", String.Format("{0}index/{1}", prefix, item.Parent.Id) },
            { "class", "parent" } },
          "Parent"
          ));
      }
      foreach (var i in item.ChildFolders) {
        folders.AppendChild(e = document.EL("li"));
        e.AppendChild(document.EL(
          "a",
          new AttributeCollection() {
            { "href", String.Format("{0}index/{1}#{2}", prefix, i.Id, i.Path) }
          },
          string.Format("{0} ({1})", i.Title, i.FullChildCount)
          ));
      }
      article.AppendChild(folders);

      var items = (XmlNode)null;
      article.AppendChild(items = document.EL(
        "ul", new AttributeCollection() { { "class", "items" } }));
      foreach (var i in item.ChildItems) {
        items.AppendChild(e = document.EL("li"));
        var link = document.EL(
          "a",
          new AttributeCollection() {
            { "href", string.Format(
              "{0}file/{1}/{2}.{3}", prefix, i.Id, i.Title,
              DlnaMaps.Dlna2Ext[i.Type][0]) }
          }
          );
        var details = document.EL("section");
        link.AppendChild(details);
        e.AppendChild(link);

        details.AppendChild(document.EL(
          "h3", new AttributeCollection { { "title", i.Title } }, i.Title));

        var props = i.Properties;
        if (props.ContainsKey("HasCover")) {
          details.AppendChild(document.EL(
            "img",
            new AttributeCollection {
              { "title", "Cover image" },
              { "alt", "Cover image" },
              { "src", String.Format(
                "{0}cover/{1}/{2}.{3}", prefix, i.Id, i.Title,
                DlnaMaps.Dlna2Ext[i.Type][0]) }
            }));
        }

        var table = document.EL("table");
        foreach (var p in htmlItemProperties) {
          string v;
          if (props.TryGetValue(p, out v)) {
            table.AppendChild(e = document.EL("tr"));
            e.AppendChild(document.EL("th", text: p));
            e.AppendChild(document.EL("td", text: v));
          }
        }
        if (table.ChildNodes.Count != 0) {
          details.AppendChild(table);
        }

        string description;
        if (props.TryGetValue("Description", out description)) {
          link.AppendChild(document.EL(
            "p", new AttributeCollection() { { "class", "desc" } },
            description));
        }
      }

      return new StringResponse(HttpCode.Ok, document.OuterXml);
    }
  }
}
