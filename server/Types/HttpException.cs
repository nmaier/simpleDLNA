using System;

namespace NMaier.SimpleDlna.Server
{
  public class HttpException : Exception
  {
    public HttpException()
    {
    }
    public HttpException(string msg)
      : base(msg)
    {
    }
  }
}
