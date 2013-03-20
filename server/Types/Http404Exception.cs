using System;

namespace NMaier.SimpleDlna.Server
{
  public class Http404Exception : HttpException
  {
    public Http404Exception()
      : base("404")
    {
    }
  }
}
