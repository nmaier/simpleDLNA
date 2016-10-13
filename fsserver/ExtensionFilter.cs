using System;
using System.Collections.Generic;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class ExtensionFilter
  {
    private static readonly StringComparer cmp = StringComparer.OrdinalIgnoreCase;

    private readonly Dictionary<string, object> exts = new Dictionary<string, object>(cmp);

    public ExtensionFilter(IEnumerable<string> extensions)
    {
      foreach (var e in extensions) {
        exts.Add(e, null);
      }
    }

    public bool Filtered(string extension)
    {
      if (string.IsNullOrEmpty(extension)) {
        return false;
      }
      return exts.ContainsKey(extension);
    }
  }
}
