using NMaier.SimpleDlna.Server.Metadata;
using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class ItemResponse : Logging, IResponse
  {
    private readonly Headers headers;

    private readonly IMediaResource item;

    private readonly HttpCode status = HttpCode.Ok;

    public ItemResponse(string prefix, IRequest request, IMediaResource item, string transferMode = "Streaming")
    {
      this.item = item;
      headers = new ResponseHeaders(noCache: !(item is IMediaCoverResource));
      var meta = item as IMetaInfo;
      if (meta != null) {
        headers.Add("Content-Length", meta.InfoSize.ToString());
        headers.Add("Last-Modified", meta.InfoDate.ToString("R"));
      }
      headers.Add("Accept-Ranges", "bytes");
      headers.Add("Content-Type", DlnaMaps.Mime[item.Type]);
      if (request.Headers.ContainsKey("getcontentFeatures.dlna.org")) {
        try {
          if (item.MediaType == DlnaMediaTypes.Image) {
            headers.Add(
              "contentFeatures.dlna.org",
              String.Format(
                "DLNA.ORG_PN={0};DLNA.ORG_OP=00;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={1}",
                item.PN,
                DlnaMaps.DefaultInteractive
                )
              );
          }
          else {
            headers.Add(
              "contentFeatures.dlna.org",
              String.Format(
                "DLNA.ORG_PN={0};DLNA.ORG_OP=01;DLNA.ORG_CI=0;DLNA.ORG_FLAGS={1}",
                item.PN,
                DlnaMaps.DefaultStreaming
                )
              );
          }
        }
        catch (NotSupportedException) {
        }
        catch (NotImplementedException) {
        }
      }
      if (request.Headers.ContainsKey("getCaptionInfo.sec")) {
        var mvi = item as IMetaVideoItem;
        if (mvi != null && mvi.Subtitle.HasSubtitle) {
          var surl = String.Format(
          "http://{0}:{1}{2}subtitle/{3}/st.srt",
          request.LocalEndPoint.Address,
          request.LocalEndPoint.Port,
          prefix,
          item.Id
          );
          DebugFormat("Sending subtitles {0}", surl);
          headers.Add("CaptionInfo.sec", surl);
        }
      }
      if (request.Headers.ContainsKey("getMediaInfo.sec")) {
        var md = item as IMetaDuration;
        if (md != null && md.MetaDuration.HasValue) {
          headers.Add(
            "MediaInfo.sec",
            string.Format(
              "SEC_Duration={0};",
              md.MetaDuration.Value.TotalMilliseconds
              )
            );
        }
      }
      headers.Add("transferMode.dlna.org", transferMode);

      Debug(headers);
    }

    public Stream Body
    {
      get
      {
        return item.CreateContentStream();
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
