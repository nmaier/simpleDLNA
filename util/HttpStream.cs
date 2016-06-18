using log4net;
using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace NMaier.SimpleDlna.Utilities
{
  public class HttpStream : Stream, IDisposable
  {
    private const int BUFFER_SIZE = 1 << 10;

    private const long SMALL_SEEK = 1 << 9;

    private const int TIMEOUT = 30000;

    private Stream bufferedStream;

    private long? length;

    private readonly static ILog logger =
      LogManager.GetLogger(typeof(HttpStream));

    public static readonly string UserAgent = GenerateUserAgent();

    private long position = 0;

    private readonly Uri referrer;

    private HttpWebRequest request;

    private HttpWebResponse response;

    private Stream responseStream;

    private readonly Uri uri;

    public HttpStream(Uri uri)
      : this(uri, null)
    {
    }

    public HttpStream(Uri uri, Uri referrer)
    {
      if (uri == null) {
        throw new ArgumentNullException("uri");
      }
      this.uri = uri;
      this.referrer = referrer;
    }

    public override bool CanRead
    {
      get
      {
        return true;
      }
    }

    public override bool CanSeek
    {
      get
      {
        if (Length <= 0) {
          return false;
        }

        EnsureResponse();
        var ranges = response.Headers.Get("Accept-Ranges");
        if (!string.IsNullOrEmpty(ranges) &&
          ranges.ToUpperInvariant() == "none") {
          return false;
        }
        return true;
      }
    }

    public override bool CanTimeout
    {
      get
      {
        return true;
      }
    }

    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }

    private void EnsureResponse()
    {
      if (response != null) {
        return;
      }
      OpenAt(0, HttpMethod.HEAD);
    }
    public string ContentType
    {
      get
      {
        EnsureResponse();
        return response.ContentType;
      }
    }

    public DateTime LastModified
    {
      get
      {
        EnsureResponse();
        return response.LastModified;
      }
    }

    public override long Length
    {
      get
      {
        if (!length.HasValue) {
          OpenAt(0, HttpMethod.HEAD);
          length = response.ContentLength;
        }
        if (length.Value < 0) {
          throw new IOException("Stream does not feature a length");
        }
        return length.Value;
      }
    }

    public override long Position
    {
      get
      {
        return position;
      }
      set
      {
        Seek(Position, SeekOrigin.Begin);
      }
    }

    public Uri Uri
    {
      get
      {
        return new Uri(uri.ToString());
      }
    }

    private static string GenerateUserAgent()
    {
      var os = Environment.OSVersion;
      var pstring = os.Platform.ToString();
      switch (os.Platform) {
        case PlatformID.Win32NT:
        case PlatformID.Win32S:
        case PlatformID.Win32Windows:
          pstring = "WIN";
          break;
        default:
          pstring = "Unix";
          break;
      }
      return String.Format(
        "sdlna/{4}.{5} ({0}{1} {2}.{3}) like curl/7.3 like wget/1.0",
        pstring,
        IntPtr.Size * 8,
        os.Version.Major,
        os.Version.Minor,
        Assembly.GetExecutingAssembly().GetName().Version.Major,
        Assembly.GetExecutingAssembly().GetName().Version.Minor
        );
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing) {
        if (bufferedStream != null) {
          bufferedStream.Dispose();
          bufferedStream = null;
        }
        if (responseStream != null) {
          responseStream.Dispose();
          responseStream = null;
        }
        response = null;
        request = null;
      }

      base.Dispose(disposing);
    }

    protected void OpenAt(long offset, HttpMethod method)
    {
      if (offset < 0) {
        throw new IOException("Position cannot be negative");
      }
      if (offset > 0 && method == HttpMethod.HEAD) {
        throw new ArgumentException(
          "Cannot use a position (seek) with HEAD request");
      }
      Close();
      Dispose();

      request = (HttpWebRequest)WebRequest.Create(uri);
      request.Method = method.ToString();
      if (referrer != null) {
        request.Referer = referrer.ToString();
      }
      request.AllowAutoRedirect = true;
      request.Timeout = TIMEOUT * 1000;
      request.UserAgent = UserAgent;
      if (offset > 0) {
        request.AddRange(offset);
      }
      response = (HttpWebResponse)request.GetResponse();
      if (method != HttpMethod.HEAD) {
        responseStream = response.GetResponseStream();
        bufferedStream = new BufferedStream(responseStream, BUFFER_SIZE);
      }
      if (offset > 0 && response.StatusCode != HttpStatusCode.PartialContent) {
        throw new IOException(
          "Failed to open the http stream at a specific position");
      }
      else {
        if (offset == 0 && response.StatusCode != HttpStatusCode.OK) {
          throw new IOException("Failed to open the http stream");
        }
      }
      logger.InfoFormat("Opened {0} {1} at {2}", method, uri, offset);
    }

    public override void Close()
    {
      if (bufferedStream != null) {
        bufferedStream.Close();
      }
      if (responseStream != null) {
        responseStream.Close();
      }
      if (response != null) {
        response.Close();
      }
      base.Close();
    }

    public new void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }

    public override void Flush()
    {
      Dispose(true);
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      try {
        if (responseStream == null) {
          OpenAt(position, HttpMethod.GET);
        }
        var read = bufferedStream.Read(buffer, offset, count);
        if (read > 0) {
          position += read;
        }
        return read;
      }
      catch (Exception ex) {
        logger.Error("Failed to read", ex);
        throw;
      }
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      logger.DebugFormat("Seek to {0}, {1} requested", offset, origin);
      var np = 0L;
      switch (origin) {
        case SeekOrigin.Begin:
          np = offset;
          break;
        case SeekOrigin.Current:
          np = position + offset;
          break;
        case SeekOrigin.End:
          np = Length + np;
          break;
      }
      if (np < 0 || np >= Length) {
        throw new IOException("Invalid seek; out of stream bounds");
      }
      var off = position - np;
      if (off == 0) {
        logger.Debug("No seek required");
      }
      else {
        if (response != null && off > 0 && off < SMALL_SEEK) {
          var buf = new byte[off];
          bufferedStream.Read(buf, 0, (int)off);
          logger.DebugFormat("Did a small seek of {0}", off);
        }
        else {
          OpenAt(np, HttpMethod.GET);
          logger.DebugFormat("Did a long seek of {0}", off);
        }
      }
      position = np;
      logger.DebugFormat("Successfully sought to {0}", position);
      return position;
    }

    public override void SetLength(long value)
    {
      length = value;
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
}
