using System;
using System.Reflection;
using System.Xml;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed partial class MediaMount : Logging, IMediaServer, IPrefixHandler
  {
    private readonly string descriptor;

    private static uint mount = 0;

    private readonly string prefix;

    private readonly IMediaServer server;

    private uint systemID = 1;


    public MediaMount(IMediaServer aServer)
    {
      server = aServer;
      prefix = String.Format("/mm-{0}/", ++mount);
      descriptor = GenerateDescriptor();
      if (server is IVolatileMediaServer) {
        (server as IVolatileMediaServer).Changed += ChangedServer;
      }
    }


    public string DescriptorURI
    {
      get
      {
        return String.Format("{0}description.xml", prefix);
      }
    }
    public string FriendlyName
    {
      get
      {
        return server.FriendlyName;
      }
    }
    public string Prefix
    {
      get
      {
        return prefix;
      }
    }
    public Guid Uuid
    {
      get
      {
        return server.Uuid;
      }
    }


    private void ChangedServer(object sender, EventArgs e)
    {
      lock (SoapCache) {
        SoapCache.Clear();
      }
      InfoFormat("Rescanned mount {0}", Uuid);
      systemID++;
    }

    private string GenerateDescriptor()
    {
      var doc = new XmlDocument();
      doc.LoadXml(Properties.Resources.description);
      doc.GetElementsByTagName("UDN").Item(0).InnerText = String.Format("uuid:{0}", Uuid);
      doc.GetElementsByTagName("modelNumber").Item(0).InnerText = Assembly.GetExecutingAssembly().GetName().Version.ToString();
      doc.GetElementsByTagName("friendlyName").Item(0).InnerText = FriendlyName + " — sdlna";
      doc.GetElementsByTagName("SCPDURL").Item(0).InnerText = String.Format("{0}contentDirectory.xml", prefix);
      doc.GetElementsByTagName("controlURL").Item(0).InnerText = String.Format("{0}control", prefix);
      doc.GetElementsByTagName("eventSubURL").Item(0).InnerText = String.Format("{0}events", prefix);

      return doc.OuterXml;
    }


    public IMediaItem GetItem(string id)
    {
      return server.GetItem(id);
    }

    public IResponse HandleRequest(IRequest request)
    {
      var path = request.Path.Substring(prefix.Length);
      Debug(path);
      if (path == "description.xml") {
        return new StringResponse(HttpCodes.OK, "text/xml", descriptor);
      }
      if (path == "contentDirectory.xml") {
        return new ResourceResponse(HttpCodes.OK, "text/xml", "contentdirectory");
      }
      if (path == "control") {
        return ProcessSoapRequest(request);
      }
      if (path.StartsWith("file/")) {
        var id = path.Split('/')[1];
        var item = GetItem(id) as IMediaResource;
        return new ItemResponse(request, item);
      }
      if (path.StartsWith("cover/")) {
        var id = path.Substring("cover/".Length);
        var item = GetItem(id) as IMediaCover;
        return new ItemResponse(request, item.Cover, "Interactive");
      }
      if (string.IsNullOrEmpty(path) || path == "index.html") {
        return new Redirect(request, prefix + "index/0");
      }
      if (path.StartsWith("index/")) {
        var id = path.Substring("index/".Length);
        var item = GetItem(id);
        return ProcessHtmlRequest(item);
      }
      if (path == "browse.css") {
        return new ResourceResponse(HttpCodes.OK, "text/css", "browse_css");
      }
      WarnFormat("Did not understand {0} {1}", request.Method, path);
      throw new Http404Exception();
    }
  }
}
