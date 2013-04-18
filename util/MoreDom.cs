using System;
using System.Xml;

namespace NMaier.SimpleDlna.Utilities
{
  public static class MoreDom
  {
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
    public static XmlElement EL(this XmlDocument doc, string name, AttributeCollection attributes = null, string text = null)
    {
      if (doc == null) {
        throw new ArgumentNullException("doc");
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
