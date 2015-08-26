using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace NMaier.SimpleDlna.Utilities
{
  public class RepositoryBase
  {
    static readonly Assembly[] _assemblies;
    static RepositoryBase() {
      //Collect all assemblies in work path
      var mask = string.Format("{0}.*.dll", Assembly.GetExecutingAssembly().GetName().Name.Split('.')[0]);
      var additional = Directory.GetFiles(".", mask).Select(p => Assembly.UnsafeLoadFrom(p)).ToArray();
      _assemblies = AppDomain.CurrentDomain.GetAssemblies()
        .Concat(additional)
        .Distinct()
        .ToArray();
    }

    public static Assembly[] GetDomainAssemblies() {
      return _assemblies;
    }

    protected static IEnumerable<Type> GetInterfaceImplementations<TInterface>()
    {
      var iface = typeof(TInterface);
      return _assemblies.SelectMany(a => a.GetTypes()).Where(t => iface.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract);
    }

  }
}
