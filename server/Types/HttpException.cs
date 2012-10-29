using System;

namespace NMaier.SimpleDlna.Server
{
  public class HttpException : Exception
  {

    public HttpException(string msg) : base(msg) { }

    public HttpException() : base() { }
  }

  public class Http404Exception : HttpException
  {

    public Http404Exception() : base("404") { }
  }
}
