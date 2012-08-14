using System.IO;
using System.Text;

namespace NMaier.sdlna.Server
{
  internal class StringResponse : IResponse
  {

    private readonly string body;
    private readonly IHeaders headers = new ResponseHeaders();
    private readonly HttpCodes status;



    public StringResponse(HttpCodes aStatus, string aMime, string aBody)
    {
      status = aStatus;
      body = aBody;

      headers["Content-Type"] = aMime;
      headers["Content-Length"] = Encoding.UTF8.GetByteCount(body).ToString();
    }

    public StringResponse(HttpCodes aStatus, string aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }



    public Stream Body
    {
      get { return new MemoryStream(Encoding.UTF8.GetBytes(body)); }
    }

    public IHeaders Headers
    {
      get { return headers; }
    }

    public HttpCodes Status
    {
      get { return status; }
    }
  }
}