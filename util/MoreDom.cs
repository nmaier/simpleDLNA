using System;
using System.Xml;

namespace NMaier.SimpleDlna.Utilities
{
  public static class MoreDom
  {
    public static XmlElement EL(this XmlDocument doc, string name)
    {
      return EL(doc, name, null, null);
    }

    public static XmlElement EL(this XmlDocument doc, string name,
      AttributeCollection attributes)
    {
      return EL(doc, name, attributes, null);
    }

    public static XmlElement EL(this XmlDocument doc, string name, string text)
    {
      return EL(doc, name, null, text);
    }

    public static XmlElement EL(this XmlDocument doc, string name,
      AttributeCollection attributes, string text)
    {
      if (doc == null) {
        throw new ArgumentNullException(nameof(doc));
      }
      var rv = doc.CreateElement(name);
      if (text != null) {
        rv.InnerText = text;
      }
      if (attributes != null) {
        foreach (var i in attributes) {
          rv.SetAttribute(i.Key, i.Value);
        }
      }
      return rv;
    }
  }
}
