namespace NMaier.SimpleDlna.Server
{
  public enum HttpCode : int
  {
    None = 0,
    Denied = 403,
    InternalError = 500,
    MovedPermanently = 301,
    NotFound = 404,
    NotModified = 304,
    Ok = 200,
    Partial = 206,
    RangeNotSatisfiable = 416,
    TemporaryRedirect = 307
  }
}
