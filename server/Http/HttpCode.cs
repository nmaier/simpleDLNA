﻿namespace NMaier.SimpleDlna.Server.Http
{
  public enum HttpCode : int
  {
    None = 0,
    Ok = 200,
    Partial = 206,
    MovedPermanently = 301,
    NotModified = 304,
    TemporaryRedirect = 307,
    Denied = 403,
    NotFound = 404,
    RangeNotSatisfiable = 416,
    InternalError = 500
  }
}
