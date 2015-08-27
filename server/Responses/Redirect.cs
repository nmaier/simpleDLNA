﻿using NMaier.SimpleDlna.Server.Http;
using System;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class Redirect : StringResponse, IResponse
  {
    internal Redirect(string uri)
      : this(HttpCode.TemporaryRedirect, uri)
    {
    }

    internal Redirect(Uri uri)
      : this(HttpCode.TemporaryRedirect, uri)
    {
    }

    internal Redirect(IRequest request, string path)
      : this(HttpCode.TemporaryRedirect, request, path)
    {
    }

    internal Redirect(HttpCode code, string uri)
      : base(code, "text/plain", "Redirecting...")
    {
      Headers.Add("Location", uri);
    }

    internal Redirect(HttpCode code, Uri uri)
      : this(code, uri.AbsoluteUri)
    {
    }

    internal Redirect(HttpCode code, IRequest request, string path)
      : this(code, string.Format("http://{0}{1}", request.LocalEndPoint, path))
    {
    }
  }
}
