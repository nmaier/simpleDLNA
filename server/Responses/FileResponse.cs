using System.IO;

namespace NMaier.sdlna.Server
{
  internal sealed class FileResponse : IResponse
  {

    private readonly FileInfo body;
    private readonly IHeaders headers = new ResponseHeaders();
    private readonly HttpCodes status;



    public FileResponse(HttpCodes aStatus, string aMime, FileInfo aBody)
    {
      status = aStatus;
      body = aBody;

      headers["Content-Type"] = aMime;
      headers["Content-Length"] = body.Length.ToString();
    }

    public FileResponse(HttpCodes aStatus, FileInfo aBody)
      : this(aStatus, "text/html; charset=utf-8", aBody)
    {
    }



    public Stream Body
    {
      get { return body.OpenRead(); }
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