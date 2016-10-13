using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace NMaier.SimpleDlna.Utilities
{
  using Attribute = KeyValuePair<string, string>;

  public class AttributeCollection : IEnumerable<Attribute>
  {
    private readonly IList<Attribute> list = new List<Attribute>();

    public int Count => list.Count;

    public ICollection<string> Keys => (from i in list
                                        select i.Key).ToList();

    public ICollection<string> Values => (from i in list
                                          select i.Value).ToList();

    IEnumerator IEnumerable.GetEnumerator()
    {
      return list.GetEnumerator();
    }

    public IEnumerator<Attribute> GetEnumerator()
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

    public bool Has(string key)
    {
      return Has(key, StringComparer.CurrentCultureIgnoreCase);
    }

    public bool Has(string key, StringComparer comparer)
    {
      return list.Any(e => comparer.Equals(key, e.Key));
    }

    public IEnumerable<string> GetValuesForKey(string key)
    {
      return GetValuesForKey(key, StringComparer.CurrentCultureIgnoreCase);
    }

    public IEnumerable<string> GetValuesForKey(string key, StringComparer comparer)
    {
      return from i in list
             where comparer.Equals(i.Key, key)
             select i.Value;
    }
  }
}
