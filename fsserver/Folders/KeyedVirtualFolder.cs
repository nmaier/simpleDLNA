using System.Collections.Generic;

namespace NMaier.SimpleDlna.FileMediaServer.Folders
{
  internal class KeyedVirtualFolder<T> : VirtualFolder where T : VirtualFolder, new()
  {

    private readonly Dictionary<string, T> keys = new Dictionary<string, T>();



    protected KeyedVirtualFolder(FileServer server, BaseFolder aParent, string aName)
      : base(server, aParent, aName)
    {
    }

    protected KeyedVirtualFolder() : this(null, null, null) { }




    public T GetFolder(string key)
    {
      T rv;
      var lkey = key.ToLower();
      if (!keys.TryGetValue(lkey, out rv)) {
        rv = new T();
        rv.Server = Server;
        rv.Name = key;
        AdoptItem(rv);
        keys.Add(lkey, rv);
      }
      return rv;
    }
  }
}
