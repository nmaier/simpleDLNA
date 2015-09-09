using NMaier.SimpleDlna.Server.Http;
using System.IO;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class FileResponse : IResponse
  {
    private readonly FileInfo body;

    private readonly IHeaders headers = new ResponseHeaders();

    private readonly HttpCode status;

    public FileResponse(HttpCode aStatus, FileInfo aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }

    public FileResponse(HttpCode aStatus, string aMime, FileInfo aBody)
    {
      status = aStatus;
      body = aBody;

      headers["Content-Type"] = aMime;
      headers["Content-Length"] = body.Length.ToString();
    }

    public Stream Body
    {
      get
      {
        return body.OpenRead();
      }
    }

    public IHeaders Headers
    {
      get
      {
        return headers;
      }
    }

    public HttpCode Status
    {
      get
      {
        return status;
      }
    }
  }
}
