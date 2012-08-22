using System;
using System.IO;
using NMaier.sdlna.Server.Metadata;

namespace NMaier.sdlna.Server
{
  internal class ItemResponse : Logging, IResponse
  {

    private readonly Headers headers = new ResponseHeaders();
    private readonly IMediaResource item;
    private HttpCodes status = HttpCodes.OK;
    private readonly Stream stream;



    public ItemResponse(IRequest request, IMediaResource aItem, string transferMode = "Streaming")
    {
      item = aItem;
      var meta = item as IMetaInfo;
      if (meta != null) {
        headers.Add("Content-Length", meta.Size.ToString());
        headers.Add("Last-Modified", meta.Date.ToString("R"));
      }
      headers.Add("Accept-Ranges", "bytes");
      headers.Add("Content-Type", DlnaMaps.Mime[item.Type]);
      if (request.Headers.ContainsKey("getcontentFeatures.dlna.org")) {
        if (item.Type == DlnaTypes.JPEG) {
          headers.Add("contentFeatures.dlna.org", String.Format("{0};DLNA.ORG_OP=00;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=00D00000000000000000000000000000", item.PN));
        }
        else {
          headers.Add("contentFeatures.dlna.org", String.Format("{0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS=01500000000000000000000000000000", item.PN));
        }
      }
      Headers.Add("transferMode.dlna.org", transferMode);


      if (request.Method == "HEAD") {
        stream = null;
      }
      else {
        stream = item.Content;
      }
      Debug(headers);
    }



    public Stream Body
    {
      get { return stream; }
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
