using System.Collections.Generic;

namespace NMaier.sdlna.Util
{
  public sealed class ResList : List<KeyValuePair<string, string>>
  {


    public void Add(string k, string v)
    {
      Add(new KeyValuePair<string, string>(k, v));
    }
  }
}
