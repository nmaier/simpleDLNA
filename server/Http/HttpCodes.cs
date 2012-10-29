using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server
{
  internal enum HttpCodes : uint
  {
    OK = 200,
    PARTIAL = 206,
    MOVED_PERMANENTLY = 301,
    NOT_MODIFIED = 304,
    TEMPORARY_REDIRECT = 307,
    DENIED = 403,
    NOT_FOUND = 404,
    RANGE_NOT_SATISFIABLE = 416,
    INTERNAL_ERROR = 500
  }

  internal static class HttpPhrases
  {

    public static readonly IDictionary<HttpCodes, string> Phrases = new Dictionary<HttpCodes, string>() {
      { HttpCodes.OK, "OK" },
      { HttpCodes.PARTIAL, "Partial Content" },
      { HttpCodes.MOVED_PERMANENTLY, "Moved Permanently" },
      { HttpCodes.NOT_MODIFIED, "Not Modified" },
      { HttpCodes.TEMPORARY_REDIRECT, "Temprary Redirect" },
      { HttpCodes.DENIED, "Access Denied" },
      { HttpCodes.NOT_FOUND, "Not Found" },
      { HttpCodes.RANGE_NOT_SATISFIABLE, "Requested Range not satisfiable" },
      { HttpCodes.INTERNAL_ERROR, "Internal Server Error" },
    };
  }
}
