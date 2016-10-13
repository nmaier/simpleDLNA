using System.Collections.Generic;
using System.Xml;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal partial class MediaMount
  {
    private readonly List<string> htmlItemProperties = new List<string>
    {
      "Type",
      "Duration",
      "Resolution",
      "Director",
      "Actors",
      "Performer",
      "Album",
      "Genre",
      "Date",
      "Size"
    };

    private IResponse ProcessHtmlRequest(IMediaItem aItem)
    {
      var item = aItem as IMediaFolder;
      if (item == null) {
        throw new HttpStatusException(HttpCode.NotFound);
      }

      var article = HtmlTools.CreateHtmlArticle(
        $"Folder: {item.Title}");
      var document = article.OwnerDocument;
      if (document == null) {
        throw new HttpStatusException(HttpCode.InternalError);
      }

      XmlNode e;
      var folders = document.EL(
        "ul",
        new AttributeCollection {{"class", "folders"}}
        );
      if (item.Parent != null) {
        folders.AppendChild(e = document.EL("li"));
        e.AppendChild(document.EL(
          "a",
          new AttributeCollection
          {
            {"href", $"{Prefix}index/{item.Parent.Id}"},
            {"class", "parent"}
          },
          "Parent"
                        ));
      }
      foreach (var i in item.ChildFolders) {
        folders.AppendChild(e = document.EL("li"));
        e.AppendChild(document.EL(
          "a",
          new AttributeCollection
          {
            {"href", $"{Prefix}index/{i.Id}#{i.Path}"}
          },
          $"{i.Title} ({i.FullChildCount})"));
      }
      article.AppendChild(folders);

      XmlNode items;
      article.AppendChild(items = document.EL(
        "ul", new AttributeCollection {{"class", "items"}}));
      foreach (var i in item.ChildItems) {
        items.AppendChild(e = document.EL("li"));
        var link = document.EL(
          "a",
          new AttributeCollection
          {
            {
              "href", $"{Prefix}file/{i.Id}/{i.Title}.{DlnaMaps.Dlna2Ext[i.Type][0]}"
            }
          }
          );
        var details = document.EL("section");
        link.AppendChild(details);
        e.AppendChild(link);

        details.AppendChild(document.EL(
          "h3", new AttributeCollection {{"title", i.Title}}, i.Title));

        var props = i.Properties;
        if (props.ContainsKey("HasCover")) {
          details.AppendChild(document.EL(
            "img",
            new AttributeCollection
            {
              {"title", "Cover image"},
              {"alt", "Cover image"},
              {
                "src", $"{Prefix}cover/{i.Id}/{i.Title}.{DlnaMaps.Dlna2Ext[i.Type][0]}"
              }
            }));
        }

        var table = document.EL("table");
        foreach (var p in htmlItemProperties) {
          string v;
          if (props.TryGetValue(p, out v)) {
            table.AppendChild(e = document.EL("tr"));
            e.AppendChild(document.EL("th", p));
            e.AppendChild(document.EL("td", v));
          }
        }
        if (table.ChildNodes.Count != 0) {
          details.AppendChild(table);
        }

        string description;
        if (props.TryGetValue("Description", out description)) {
          link.AppendChild(document.EL(
            "p", new AttributeCollection {{"class", "desc"}},
            description));
        }
      }

      return new StringResponse(HttpCode.Ok, document.OuterXml);
    }
  }
}
