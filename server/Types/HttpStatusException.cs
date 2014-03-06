using System;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.Server
{
  [Serializable]
  public class HttpStatusException : HttpException
  {
    protected HttpStatusException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
    }

    private HttpStatusException() {
    }


    public HttpStatusException(HttpCodes code)
      : base(HttpPhrases.Phrases[code])
    {
      Code = code;
    }
    public HttpStatusException(HttpCodes code, Exception innerException)
      : base(HttpPhrases.Phrases[code], innerException)
    {
      Code = code;
    }

    public HttpCodes Code
    {
      get;
      private set;
    }
  }
}
