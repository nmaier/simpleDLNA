using System;

namespace NMaier.sdlna.Server
{
  internal sealed class Redirect : StringResponse, IResponse
  {

    internal Redirect(HttpCodes code, IRequest request, string path)
      : this(code, string.Format("http://{0}{1}", request.LocalEndPoint, path))
    { }

    internal Redirect(IRequest request, string path)
      : this(HttpCodes.TEMPORARY_REDIRECT, request, path)
    { }

    internal Redirect(HttpCodes code, string uri)
      : base(code, "text/plain", "Redirecting...")
    {
      Headers.Add("Location", uri);
    }

    internal Redirect(HttpCodes code, Uri uri)
      : this(code, uri.AbsoluteUri)
    { }

    internal Redirect(string uri)
      : this(HttpCodes.TEMPORARY_REDIRECT, uri)
    { }

    internal Redirect(Uri uri)
      : this(HttpCodes.TEMPORARY_REDIRECT, uri)
    { }
  }
}
