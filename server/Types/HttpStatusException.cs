using System;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public sealed class HttpStatusException : HttpException
  {
    private HttpStatusException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    public HttpStatusException()
    {
    }

    public HttpStatusException(HttpCode code)
      : base(HttpPhrases.Phrases[code])
    {
      Code = code;
    }

    public HttpStatusException(string message)
      : base(message)
    {
      Code = HttpCode.None;
    }

    public HttpStatusException(HttpCode code, Exception innerException)
      : base(HttpPhrases.Phrases[code], innerException)
    {
      Code = code;
    }

    public HttpStatusException(string message, Exception innerException)
      : base(message, innerException)
    {
      Code = HttpCode.None;
    }

    public HttpCode Code { get; private set; }
  }
}
