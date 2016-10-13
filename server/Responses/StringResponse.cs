using System.IO;
using System.Text;

namespace NMaier.SimpleDlna.Server
{
  internal class StringResponse : IResponse
  {
    private readonly string body;

    public StringResponse(HttpCode aStatus, string aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }

    public StringResponse(HttpCode aStatus, string aMime, string aBody)
    {
      Status = aStatus;
      body = aBody;

      Headers["Content-Type"] = aMime;
      Headers["Content-Length"] = Encoding.UTF8.GetByteCount(body).ToString();
    }

    public Stream Body => new MemoryStream(Encoding.UTF8.GetBytes(body));

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
  }
}
