using System.Collections.Generic;

namespace NMaier.sdlna.Server
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
    public static readonly IDictionary<HttpCodes, string> Phrases = new Dictionary<HttpCodes, string>();

    static HttpPhrases()
    {
      Phrases.Add(HttpCodes.OK, "OK");
      Phrases.Add(HttpCodes.PARTIAL, "Partial Content");
      Phrases.Add(HttpCodes.MOVED_PERMANENTLY, "Moved Permanently");
      Phrases.Add(HttpCodes.NOT_MODIFIED, "Not Modified");
      Phrases.Add(HttpCodes.TEMPORARY_REDIRECT, "Temprary Redirect");
      Phrases.Add(HttpCodes.DENIED, "Access Denied");
      Phrases.Add(HttpCodes.NOT_FOUND, "Not Found");
      Phrases.Add(HttpCodes.RANGE_NOT_SATISFIABLE, "Requested Range not satisfiable");
      Phrases.Add(HttpCodes.INTERNAL_ERROR, "Internal Server Error");
    }
  }
}
