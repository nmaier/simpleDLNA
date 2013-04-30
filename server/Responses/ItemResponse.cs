using System;
using System.IO;
using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class ItemResponse : Logging, IResponse
  {
    private readonly Headers headers;

    private readonly HttpCodes status = HttpCodes.OK;

    private readonly IMediaResource item;


    public ItemResponse(IRequest request, IMediaResource aItem, string transferMode = "Streaming")
    {
      item = aItem;
      headers = new ResponseHeaders(noCache: !(item is IMediaCoverResource));
      var meta = item as IMetaInfo;
      if (meta != null) {
        headers.Add("Content-Length", meta.InfoSize.ToString());
        headers.Add("Last-Modified", meta.InfoDate.ToString("R"));
      }
      headers.Add("Accept-Ranges", "bytes");
      headers.Add("Content-Type", DlnaMaps.Mime[item.Type]);
      if (request.Headers.ContainsKey("getcontentFeatures.dlna.org")) {
        if (item.Type == DlnaMime.JPEG) {
          headers.Add("contentFeatures.dlna.org", String.Format("{0};DLNA.ORG_OP=00;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=00D00000000000000000000000000000", item.PN));
        }
        else {
          headers.Add("contentFeatures.dlna.org", String.Format("{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=01500000000000000000000000000000", item.PN));
        }
      }
      Headers.Add("transferMode.dlna.org", transferMode);

      Debug(headers);
    }


    public Stream Body
    {
      get
      {
        return item.Content;
      }
    }
    public IHeaders Headers
    {
      get
      {
        return headers;
      }
    }
    public HttpCodes Status
    {
      get
      {
        return status;
      }
    }
  }
}
