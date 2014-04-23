using System.Collections.Generic;
using System.IO;

namespace NMaier.SimpleDlna.Server
{
  public interface IHeaders : IDictionary<string, string>
  {
    string HeaderBlock { get; }

    Stream HeaderStream { get; }
  }
}
