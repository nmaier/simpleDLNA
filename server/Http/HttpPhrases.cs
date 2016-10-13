using System.Collections.Generic;

namespace NMaier.SimpleDlna.Server
{
  internal static class HttpPhrases
  {
    public static readonly IDictionary<HttpCode, string> Phrases =
      new Dictionary<HttpCode, string>
      {
        {HttpCode.Ok, "OK"},
        {HttpCode.Partial, "Partial Content"},
        {HttpCode.MovedPermanently, "Moved Permanently"},
        {HttpCode.NotModified, "Not Modified"},
        {HttpCode.TemporaryRedirect, "Temprary Redirect"},
        {HttpCode.Denied, "Forbidden"},
        {HttpCode.NotFound, "Not Found"},
        {HttpCode.RangeNotSatisfiable, "Requested Range not satisfiable"},
        {HttpCode.InternalError, "Internal Server Error"}
      };
  }
}
