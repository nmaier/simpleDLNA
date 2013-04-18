namespace NMaier.SimpleDlna.Server
{
  internal enum HttpCodes : uint
  {
    DENIED = 403,
    INTERNAL_ERROR = 500,
    MOVED_PERMANENTLY = 301,
    NOT_FOUND = 404,
    NOT_MODIFIED = 304,
    OK = 200,
    PARTIAL = 206,
    RANGE_NOT_SATISFIABLE = 416,
    TEMPORARY_REDIRECT = 307
  }
}
