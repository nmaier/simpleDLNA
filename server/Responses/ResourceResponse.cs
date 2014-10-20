using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Resources;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class ResourceResponse : Logging, IResponse
  {
    private readonly IHeaders headers = new ResponseHeaders();

    private readonly byte[] resource;

    private readonly HttpCode status;

    public ResourceResponse(HttpCode aStatus, string type, string aResource)
      : this(aStatus, type, Properties.Resources.ResourceManager, aResource)
    {
    }

    public ResourceResponse(HttpCode aStatus, string type,
                            ResourceManager aResourceManager, string aResource)
    {
      status = aStatus;
      try {
        resource = aResourceManager.GetObject(aResource) as byte[];

        headers["Content-Type"] = type;
        headers["Content-Length"] = resource.Length.ToString();
      }
      catch (Exception ex) {
        Error("Failed to prepare resource " + aResource, ex);
        throw;
      }
    }

    public Stream Body
    {
      get
      {
        return new MemoryStream(resource);
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
