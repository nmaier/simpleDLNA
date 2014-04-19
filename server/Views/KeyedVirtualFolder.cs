using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server.Views
{
  internal class KeyedVirtualFolder<T> : VirtualFolder
    where T : VirtualFolder, new()
  {
    private readonly Dictionary<string, T> keys = new Dictionary<string, T>();

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
      var lkey = key.ToUpper();
      if (!keys.TryGetValue(lkey, out rv)) {
        rv = new T();
        rv.Name = key;
        rv.Parent = this;
        folders.Add(rv);
        keys.Add(lkey, rv);
      }
      return rv;
    }
  }
}
