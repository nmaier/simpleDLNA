using System.IO;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class FileResponse : IResponse
  {
    private readonly FileInfo body;

    public FileResponse(HttpCode aStatus, FileInfo aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }

    public FileResponse(HttpCode aStatus, string aMime, FileInfo aBody)
    {
      Status = aStatus;
      body = aBody;

      Headers["Content-Type"] = aMime;
      Headers["Content-Length"] = body.Length.ToString();
    }

    public Stream Body => body.OpenRead();

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
  }
}
