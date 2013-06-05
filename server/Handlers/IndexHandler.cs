using System.Xml;
using NMaier.SimpleDlna.Utilities;
using System.Reflection;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class IndexHandler : IPrefixHandler
  {
    private readonly HttpServer owner;

    public string Prefix
    {
      get
      {
        return "/";
      }
    }

    public IndexHandler(HttpServer owner)
    {
      this.owner = owner;
    }

    public IResponse HandleRequest(IRequest req)
    {
      var doc = new XmlDocument();
      doc.AppendChild(doc.CreateDocumentType("html", null, null, null));

      doc.AppendChild(doc.EL("html"));

      var head = doc.EL("head");
      doc.DocumentElement.AppendChild(head);
      head.AppendChild(doc.EL("title", text: "simple DLNA"));
      head.AppendChild(doc.EL(
        "link",
        new AttributeCollection() { { "rel", "stylesheet" }, { "type", "text/css" }, { "href", "/static/browse.css" } }
        ));

      var body = doc.EL("body");
      doc.DocumentElement.AppendChild(body);
      var article = doc.EL("article");
      body.AppendChild(article);

      article.AppendChild(doc.EL("h1", text: "Simple DLNA"));

      var list = doc.EL("ul");
      foreach (var m in owner.MediaMounts) {
        var li = doc.EL("li");
        li.AppendChild(doc.EL("a", new AttributeCollection() {{ "href", m.Key}}, m.Value));
        list.AppendChild(li);
      }

      article.AppendChild(list);

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
