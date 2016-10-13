using System;
using System.IO;
using System.Resources;
using NMaier.SimpleDlna.Server.Properties;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class ResourceResponse : Logging, IResponse
  {
    private readonly byte[] resource;

    public ResourceResponse(HttpCode aStatus, string type, string aResource)
      : this(aStatus, type, Resources.ResourceManager, aResource)
    {
    }

    public ResourceResponse(HttpCode aStatus, string type,
      ResourceManager aResourceManager, string aResource)
    {
      Status = aStatus;
      try {
        resource = (byte[])aResourceManager.GetObject(aResource);

        Headers["Content-Type"] = type;
        var len = resource?.Length.ToString() ?? "0";
        Headers["Content-Length"] = len;
      }
      catch (Exception ex) {
        Error("Failed to prepare resource " + aResource, ex);
        throw;
      }
    }

    public Stream Body => new MemoryStream(resource);

    public IHeaders Headers { get; } = new ResponseHeaders();

    public HttpCode Status { get; }
  }
}
