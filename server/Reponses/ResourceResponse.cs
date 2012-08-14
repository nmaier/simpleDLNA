using System.IO;
using System.Resources;

namespace NMaier.sdlna.Server
{
  internal class ResourceResponse : IResponse
  {

    private readonly IHeaders headers = new ResponseHeaders();
    private readonly byte[] resource;
    private readonly HttpCodes status;



    public ResourceResponse(HttpCodes aStatus, string type, ResourceManager aResourceManager, string aResource)
    {

      status = aStatus;
      resource = aResourceManager.GetObject(aResource) as byte[];

      headers["Content-Type"] = type;
      headers["Content-Length"] = resource.Length.ToString();
    }

    public ResourceResponse(HttpCodes aStatus, string type, string aResource)
      : this(aStatus, type, Properties.Resources.ResourceManager, aResource)
    {
    }



    public Stream Body
    {
      get { return new MemoryStream(resource); }
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
