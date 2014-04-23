using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Utilities
{
  public abstract class Repository<TInterface>
    where TInterface : class, IRepositoryItem
  {
    private static readonly Dictionary<string, TInterface> items =
      BuildRepository();

    private static Dictionary<string, TInterface> BuildRepository()
    {
      var items = new Dictionary<string, TInterface>();
      var type = typeof(TInterface).Name;
      var a = typeof(TInterface).Assembly;
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface(type) == null) {
          continue;
        }
        var ctor = t.GetConstructor(new Type[] { });
        if (ctor == null) {
          continue;
        }
        try {
          var item = ctor.Invoke(new object[] { }) as TInterface;
          if (item == null) {
            continue;
          }
          items.Add(item.Name.ToUpperInvariant(), item);
        }
        catch (Exception) {
          continue;
        }
      }
      return items;
    }

    public static IDictionary<string, IRepositoryItem> ListItems()
    {
      var rv = new Dictionary<string, IRepositoryItem>();
      foreach (var v in items.Values) {
        rv.Add(v.Name, v);
      }
      return rv;
    }

    public static TInterface Lookup(string name)
    {
      if (string.IsNullOrWhiteSpace(name)) {
        throw new ArgumentException(
          "Invalid repository name",
          "name");
      }
      var n_p = name.Split(new char[] { ':' }, 2);
      name = n_p[0].ToUpperInvariant().Trim();
      var result = (TInterface)null;
      if (!items.TryGetValue(name, out result)) {
        throw new RepositoryLookupException(name);
      }
      if (n_p.Length == 1) {
        return result;
      }

      var ctor = result.GetType().GetConstructor(new Type[] { });
      if (ctor == null) {
        throw new RepositoryLookupException(name);
      }
      var parameters = new AttributeCollection();
      foreach (var p in n_p[1].Split(',')) {
        var k_v = p.Split(new char[] { '=' }, 2);
        if (k_v.Length == 2) {
          parameters.Add(k_v[0], k_v[1]);
        }
        else {
          parameters.Add(k_v[0], null);
        }
      }
      try {
        var item = ctor.Invoke(new object[] { }) as TInterface;
        if (item == null) {
          throw new RepositoryLookupException(name);
        }
        item.SetParameters(parameters);
        return item;
      }
      catch (Exception ex) {
        throw new RepositoryLookupException(string.Format(
          "Cannot construct repository item: {0}",
          ex.Message), ex);
      }
    }
  }
}
