using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server
{
  public class Headers : IHeaders
  {

    private readonly bool asIs = false;
    private Dictionary<string, string> dict = new Dictionary<string, string>();
    static private Regex validator = new Regex(
      @"^[a-z\d][a-z\d_.-]+$",
      RegexOptions.Compiled | RegexOptions.IgnoreCase
      );



    protected Headers(bool asIs = false)
    {
      this.asIs = asIs;
    }

    public Headers() : this(false) { }



    public int Count
    {
      get { return dict.Count; }
    }

    public string HeaderBlock
    {
      get
      {
        var hb = new StringBuilder();
        foreach (var h in this) {
          hb.AppendFormat("{0}: {1}\r\n", h.Key, h.Value);
        }
        return hb.ToString();
      }
    }

    public Stream HeaderStream
    {
      get
      {
        return new MemoryStream(Encoding.ASCII.GetBytes(HeaderBlock));
      }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public ICollection<string> Keys
    {
      get { return dict.Keys; }
    }

    public string this[string key]
    {
      get
      {
        return dict[Normalize(key)];
      }
      set
      {
        dict[Normalize(key)] = value;
      }
    }

    public ICollection<string> Values
    {
      get { return dict.Values; }
    }




    public void Add(string key, string value)
    {
      dict.Add(Normalize(key), value);
    }

    public void Add(KeyValuePair<string, string> item)
    {
      Add(item.Key, item.Value);
    }

    public void Clear()
    {
      dict.Clear();
    }

    public bool Contains(KeyValuePair<string, string> item)
    {
      var p = new KeyValuePair<string, string>(Normalize(item.Key), item.Value);
      return dict.Contains(p);
    }

    public bool ContainsKey(string key)
    {
      return dict.ContainsKey(Normalize(key));
    }

    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
    {
      throw new NotImplementedException();
    }

    public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
    {
      return dict.GetEnumerator();
    }

    public bool Remove(string key)
    {
      return dict.Remove(Normalize(key));
    }

    public bool Remove(KeyValuePair<string, string> item)
    {
      return Remove(item.Key);
    }

    public bool TryGetValue(string key, out string value)
    {
      return dict.TryGetValue(Normalize(key), out value);
    }

    private string Normalize(string header)
    {
      if (!asIs) {
        header = header.ToLower();
      }
      header = header.Trim();
      if (!validator.IsMatch(header)) {
        throw new ArgumentException("Invalid header: " + header);
      }
      return header;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
      return dict.GetEnumerator();
    }
  }
}