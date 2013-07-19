using System;
using System.Collections.Generic;
using System.Linq;

namespace NMaier.SimpleDlna.Utilities
{
  using Attribute = KeyValuePair<string, string>;

  public sealed class AttributeCollection : IEnumerable<Attribute>
  {
    private readonly IList<Attribute> list = new List<Attribute>();


    public int Count
    {
      get
      {
        return list.Count;
      }
    }
    public ICollection<string> Keys
    {
      get
      {
        return (from i in list
                select i.Key).ToList();
      }
    }
    public ICollection<string> Values
    {
      get
      {
        return (from i in list
                select i.Value).ToList();
      }
    }


    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return list.GetEnumerator();
    }


    public void Add(Attribute item)
    {
      list.Add(item);
    }

    public void Add(string key, string value)
    {
      list.Add(new Attribute(key, value));
    }

    public void Clear()
    {
      list.Clear();
    }

    public bool Contains(Attribute item)
    {
      return list.Contains(item);
    }

    public IEnumerator<Attribute> GetEnumerator()
    {
      return list.GetEnumerator();
    }

    public IEnumerable<string> GetValuesForKey(string key)
    {
      return from i in list
             where StringComparer.CurrentCultureIgnoreCase.Equals(i.Key, key)
             select i.Value;
    }
  }
}
