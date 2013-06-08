using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class IndexHandler : IPrefixHandler
  {
    private readonly HttpServer owner;


    public IndexHandler(HttpServer owner)
    {
      this.owner = owner;
    }


    public string Prefix
    {
      get
      {
        return "/";
      }
    }


    public IResponse HandleRequest(IRequest req)
    {
      var article = HtmlTools.CreateHtmlArticle("Index");
      var document = article.OwnerDocument;

      var list = document.EL("ul");
      foreach (var m in owner.MediaMounts) {
        var li = document.EL("li");
        li.AppendChild(document.EL("a", new AttributeCollection() { { "href", m.Key } }, m.Value));
        list.AppendChild(li);
      }

      article.AppendChild(list);

      return new StringResponse(HttpCodes.OK, document.OuterXml);
    }
  }
}
