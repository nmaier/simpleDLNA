using System;
using System.Collections.Generic;
using System.Reflection;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public abstract class Repository<TInterface> where TInterface : class, IRepositoryItem
  {

    private static readonly Dictionary<string, TInterface> items = new Dictionary<string, TInterface>();



    static Repository()
    {
      var type = typeof(TInterface).Name;
      var a = Assembly.GetExecutingAssembly();
      foreach (Type t in a.GetTypes()) {
        if (t.GetInterface(type) == null) {
          continue;
        }
        ConstructorInfo ctor = t.GetConstructor(new Type[] { });
        if (ctor == null) {
          continue;
        }
        try {
          var item = ctor.Invoke(new object[] { }) as TInterface;
          if (item == null) {
            continue;
          }
          items.Add(item.Name, item);
        }
        catch (Exception) { }
      }
    }




    public static IDictionary<string, string> ListItems()
    {
      var rv = new Dictionary<string, string>();
      foreach (var v in items.Values) {
        rv.Add(v.Name, v.Description);
      }
      return rv;
    }

    public static TInterface Lookup(string name)
    {
      name = name.ToLower().Trim();
      TInterface result = null;
      if (!items.TryGetValue(name, out result)) {
        throw new RepositoryLookupException(name);
      }
      return result;
    }
  }
}
