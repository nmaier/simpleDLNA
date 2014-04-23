using System;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public class HttpException : Exception
  {
    protected HttpException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public HttpException()
    {
    }

    public HttpException(string message)
      : base(message)
    {
    }

    public HttpException(string message, Exception innerException)
      : base(message, innerException)
    {
    }
  }
}
