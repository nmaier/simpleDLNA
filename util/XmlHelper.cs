using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace NMaier.SimpleDlna.Utilities
{
  public static class XmlHelper
  {
    public static void ToFile(object obj, string filename) {
        var serializer = new XmlSerializer(obj.GetType());
        using (var writer = new StreamWriter(filename)) {
          serializer.Serialize(writer, obj);
        }
    }

    public static T FromFile<T>(string filename) {
        var serializer = new XmlSerializer(typeof(T));
        using (var reader = new StreamReader(filename)) {
          return (T)serializer.Deserialize(reader);
        }
    }
  }
}
