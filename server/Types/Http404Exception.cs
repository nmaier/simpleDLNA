using System;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public class Http404Exception : HttpException
  {
    protected Http404Exception(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }


    public Http404Exception()
      : base("404")
    {
    }
    public Http404Exception(string msg)
      : base(msg)
    {
    }
    public Http404Exception(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
