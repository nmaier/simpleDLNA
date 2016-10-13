using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class KeyedVirtualFolder<T> : VirtualFolder
    where T : VirtualFolder, new()
  {
    private readonly Dictionary<string, T> keys = new Dictionary<string, T>(StringComparer.CurrentCultureIgnoreCase);

    protected KeyedVirtualFolder()
      : this(null, null)
    {
    }

    protected KeyedVirtualFolder(IMediaFolder aParent, string aName)
      : base(aParent, aName)
    {
    }

    public T GetFolder(string key)
    {
      T rv;
      if (!keys.TryGetValue(key, out rv)) {
        rv = new T
        {
          Name = key,
          Parent = this
        };
        Folders.Add(rv);
        keys.Add(key, rv);
      }
      return rv;
    }
  }
}
