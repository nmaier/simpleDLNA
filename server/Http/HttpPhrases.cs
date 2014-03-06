using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server
{
  internal static class HttpPhrases
  {
    public static readonly IDictionary<HttpCodes, string> Phrases = new Dictionary<HttpCodes, string>() {
        { HttpCodes.OK, "OK" },
        { HttpCodes.PARTIAL, "Partial Content" },
        { HttpCodes.MOVED_PERMANENTLY, "Moved Permanently" },
        { HttpCodes.NOT_MODIFIED, "Not Modified" },
        { HttpCodes.TEMPORARY_REDIRECT, "Temprary Redirect" },
        { HttpCodes.DENIED, "Forbidden" },
        { HttpCodes.NOT_FOUND, "Not Found" },
        { HttpCodes.RANGE_NOT_SATISFIABLE, "Requested Range not satisfiable" },
        { HttpCodes.INTERNAL_ERROR, "Internal Server Error" } };
  }
}
