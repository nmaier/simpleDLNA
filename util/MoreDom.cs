using System.Xml;

namespace NMaier.sdlna.Util
{
  public static class MoreDom
  {


    public static XmlElement E(this XmlDocument doc, string name, string text = null)
    {
      return doc.E(name, null, text);
    }

    public static XmlElement E(this XmlDocument doc, string name, ResList attrs, string text = null)
    {
      var rv = doc.CreateElement(name);
      if (text != null) {
        rv.InnerText = text;
      }
      if (attrs != null) {
        foreach (var i in attrs) {
          rv.SetAttribute(i.Key, i.Value.ToString());
        }
      }
      return rv;
    }
  }
}
