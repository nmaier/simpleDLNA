using System.Collections.Generic;

namespace NMaier.sdlna.Server
{
  internal class ResList : List<KeyValuePair<string, string>>
  {
    public void Add(string k, string v)
    {
      Add(new KeyValuePair<string, string>(k, v));
    }
  }
}
