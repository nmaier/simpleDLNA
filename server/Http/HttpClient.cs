using NMaier.SimpleDlna.Utilities;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class HttpClient : Logging, IRequest, IDisposable
  {
    private const uint BEGIN_TIMEOUT = 30;

    private const int BUFFER_SIZE = 1 << 16;

    private const string CRLF = "\r\n";

    private string body;

    private uint bodyBytes = 0;

    private readonly byte[] buffer = new byte[2048];

    private static readonly Regex bytes =
      new Regex(@"^bytes=(\d+)(?:-(\d+)?)?$", RegexOptions.Compiled);

    private readonly static IHandler Error403 =
      new StaticHandler(new StringResponse(
        HttpCode.Denied,
        "<!doctype html><title>Access denied!</title><h1>Access denied!</h1><p>You're not allowed to access the requested resource.</p>"
        )
      );

    private readonly static IHandler Error404 =
      new StaticHandler(new StringResponse(
        HttpCode.NotFound,
        "<!doctype html><title>Not found!</title><h1>Not found!</h1><p>The requested resource was not found!</p>"
        )
      );

    private readonly static IHandler Error416 =
      new StaticHandler(new StringResponse(
        HttpCode.RangeNotSatisfiable,
        "<!doctype html><title>Requested Range not satisfiable!</title><h1>Requested Range not satisfiable!</h1><p>Nice try, but do not try again :p</p>"
        )
      );

    private readonly static IHandler Error500 =
      new StaticHandler(new StringResponse(
        HttpCode.InternalError,
        "<!doctype html><title>Internal Server Error</title><h1>Internal Server Error</h1><p>Something is very rotten in the State of Denmark!</p>"
        )
      );

    private readonly IHeaders headers = new Headers();

    private readonly uint READ_TIMEOUT =
      (uint)TimeSpan.FromMinutes(1).TotalSeconds;

    private readonly uint WRITE_TIMEOUT =
      (uint)TimeSpan.FromMinutes(180).TotalSeconds;

    private readonly TcpClient client;

    private bool hasHeaders = false;

    private DateTime lastActivity;

    private string method;

    private readonly HttpServer owner;

    private string path;

    private MemoryStream readStream;

    private uint requestCount = 0;

    private IResponse response;

    private HttpStates state;

    private readonly NetworkStream stream;

    public HttpClient(HttpServer aOwner, TcpClient aClient)
    {
      State = HttpStates.ACCEPTED;
      lastActivity = DateTime.Now;

      owner = aOwner;
      client = aClient;
      stream = client.GetStream();
      client.Client.UseOnlyOverlappedIO = true;

      RemoteEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
      LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
    }

    internal enum HttpStates
    {
      ACCEPTED,
      CLOSED,
      READBEGIN,
      READING,
      WRITEBEGIN,
      WRITING
    }

    private HttpStates State
    {
      set
      {
        lastActivity = DateTime.Now;
        state = value;
      }
    }

    public string Body
    {
      get
      {
        return body;
      }
    }

    public IHeaders Headers
    {
      get
      {
        return headers;
      }
    }

    public bool IsATimeout
    {
      get
      {
        var diff = (DateTime.Now - lastActivity).TotalSeconds;
        switch (state) {
          case HttpStates.ACCEPTED:
          case HttpStates.READBEGIN:
          case HttpStates.WRITEBEGIN:
            return diff > BEGIN_TIMEOUT;
          case HttpStates.READING:
            return diff > READ_TIMEOUT;
          case HttpStates.WRITING:
            return diff > WRITE_TIMEOUT;
          case HttpStates.CLOSED:
            return true;
          default:
            throw new InvalidOperationException("Invalid state");
        }
      }
    }

    public IPEndPoint LocalEndPoint
    {
      get;
      private set;
    }

    public string Method
    {
      get
      {
        return method;
      }
    }

    public string Path
    {
      get
      {
        return path;
      }
    }

    public IPEndPoint RemoteEndpoint
    {
      get;
      private set;
    }

    private long GetContentLengthFromStream(Stream responseBody)
    {
      long contentLength = -1;
      try {
        string clf;
        if (!response.Headers.TryGetValue("Content-Length", out clf) ||
          !long.TryParse(clf, out contentLength)) {
          contentLength = responseBody.Length - responseBody.Position;
          if (contentLength < 0) {
            throw new InvalidDataException();
          }
          response.Headers["Content-Length"] = contentLength.ToString();
        }
      }
      catch (Exception) {
      }
      return contentLength;
    }

    private Stream ProcessRanges(IResponse rangedResponse, ref HttpCode status)
    {
      var responseBody = rangedResponse.Body;
      var contentLength = GetContentLengthFromStream(responseBody);
      try {
        string ar;
        if (status != HttpCode.Ok && contentLength > 0 ||
          !headers.TryGetValue("Range", out ar)) {
          return responseBody;
        }
        var m = bytes.Match(ar);
        if (!m.Success) {
          throw new InvalidDataException("Not parsed!");
        }
        var totalLength = contentLength;
        var start = 0L;
        var end = totalLength - 1;
        if (!long.TryParse(m.Groups[1].Value, out start) || start < 0) {
          throw new InvalidDataException("Not parsed");
        }
        if (m.Groups.Count != 3 ||
            !long.TryParse(m.Groups[2].Value, out end) ||
            end <= start || end >= totalLength) {
          end = totalLength - 1;
        }
        if (start >= end) {
          responseBody.Close();
          rangedResponse = Error416.HandleRequest(this);
          return rangedResponse.Body;
        }

        if (start > 0) {
          responseBody.Seek(start, SeekOrigin.Current);
        }
        contentLength = end - start + 1;
        rangedResponse.Headers["Content-Length"] = contentLength.ToString();
        rangedResponse.Headers.Add(
          "Content-Range",
          String.Format("bytes {0}-{1}/{2}", start, end, totalLength)
        );
        status = HttpCode.Partial;
      }
      catch (Exception ex) {
        Warn(String.Format(
          "{0} - Failed to process range request!", this), ex);
      }
      return responseBody;
    }

    private void Read()
    {
      try {
        stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, 0);
      }
      catch (IOException ex) {
        Warn(String.Format("{0} - Failed to BeginRead", this), ex);
        Close();
      }
    }

    private void ReadCallback(IAsyncResult result)
    {
      if (state == HttpStates.CLOSED) {
        return;
      }

      State = HttpStates.READING;

      var read = 0;
      try {
        read = stream.EndRead(result);
        if (read < 0) {
          throw new HttpException("Client did not send anything");
        }
        DebugFormat("{0} - Read {1} bytes", this, read);
        readStream.Write(buffer, 0, read);
        lastActivity = DateTime.Now;
      }
      catch (Exception) {
        if (!IsATimeout) {
          WarnFormat("{0} - Failed to read data", this);
          Close();
        }
        return;
      }

      try {
        if (!hasHeaders) {
          readStream.Seek(0, SeekOrigin.Begin);
          var reader = new StreamReader(readStream);
          for (var line = reader.ReadLine(); line != null;
            line = reader.ReadLine()) {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) {
              hasHeaders = true;
              readStream = new MemoryStream();
              if (headers.ContainsKey("content-length") &&
                uint.TryParse(headers["content-length"], out bodyBytes)) {
                if (bodyBytes > (1 << 20)) {
                  throw new IOException("Body too long");
                }
                var bytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                readStream.Write(bytes, 0, bytes.Length);
                DebugFormat("Must read body bytes {0}", bodyBytes);
              }
              break;
            }
            if (method == null) {
              var parts = line.Split(new char[] { ' ' }, 3);
              method = parts[0].Trim().ToUpperInvariant();
              path = parts[1].Trim();
              DebugFormat("{0} - {1} request for {2}", this, method, path);
            }
            else {
              var parts = line.Split(new char[] { ':' }, 2);
              headers[parts[0]] = Uri.UnescapeDataString(parts[1]).Trim();
            }
          }
        }
        if (bodyBytes != 0 && bodyBytes > readStream.Length) {
          DebugFormat(
            "{0} - Bytes to go {1}", this, bodyBytes - readStream.Length);
          Read();
          return;
        }
        using (readStream) {
          body = Encoding.UTF8.GetString(readStream.ToArray());
          Debug(body);
          Debug(headers);
        }
        SetupResponse();
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to process request", this), ex);
        response = Error500.HandleRequest(this);
        SendResponse();
      }
    }

    private void ReadNext()
    {
      method = null;
      headers.Clear();
      hasHeaders = false;
      body = null;
      bodyBytes = 0;
      readStream = new MemoryStream();

      ++requestCount;
      State = HttpStates.READBEGIN;

      Read();
    }

    private void SendResponse()
    {
      var statusCode = response.Status;
      var responseBody = ProcessRanges(response, ref statusCode);
      var responseStream = new ConcatenatedStream();
      try {
        var headerBlock = new StringBuilder();
        headerBlock.AppendFormat(
          "HTTP/1.1 {0} {1}\r\n",
          (uint)statusCode,
          HttpPhrases.Phrases[statusCode]
        );
        headerBlock.Append(response.Headers.HeaderBlock);
        headerBlock.Append(CRLF);

        var headerStream = new MemoryStream(
          Encoding.ASCII.GetBytes(headerBlock.ToString()));
        responseStream.AddStream(headerStream);
        if (method != "HEAD" && responseBody != null) {
          responseStream.AddStream(responseBody);
          responseBody = null;
        }
        InfoFormat("{0} - {1} response for {2}", this, (uint)statusCode, path);
        state = HttpStates.WRITING;
        var sp = new StreamPump(responseStream, stream, BUFFER_SIZE);
        sp.Pump((pump, result) =>
        {
          pump.Input.Close();
          pump.Input.Dispose();
          if (result == StreamPumpResult.Delivered) {
            DebugFormat("{0} - Done writing response", this);

            string conn;
            if (headers.TryGetValue("connection", out conn) &&
              conn.ToUpperInvariant() == "KEEP-ALIVE") {
              ReadNext();
              return;
            }
          }
          else {
            DebugFormat("{0} - Client aborted connection", this);
          }
          Close();
        });
      }
      catch (Exception) {
        responseStream.Dispose();
        throw;
      }
      finally {
        if (responseBody != null) {
          responseBody.Dispose();
        }
      }
    }

    private void SetupResponse()
    {
      State = HttpStates.WRITEBEGIN;
      try {
        if (!owner.AuthorizeClient(this)) {
          throw new HttpStatusException(HttpCode.Denied);
        }
        if (string.IsNullOrEmpty(path)) {
          throw new HttpStatusException(HttpCode.NotFound);
        }
        var handler = owner.FindHandler(path);
        if (handler == null) {
          throw new HttpStatusException(HttpCode.NotFound);
        }
        response = handler.HandleRequest(this);
        if (response == null) {
          throw new ArgumentException("Handler did not return a response");
        }
      }
      catch (HttpStatusException ex) {
#if DEBUG
        Warn(String.Format("{0} - Got a {2}: {1}", this, path, ex.Code), ex);
#else
        InfoFormat("{0} - Got a {2}: {1}", this, path, ex.Code);
#endif
        switch (ex.Code) {
          case HttpCode.NotFound:
            response = Error404.HandleRequest(this);
            break;
          case HttpCode.Denied:
            response = Error403.HandleRequest(this);
            break;
          case HttpCode.InternalError:
            response = Error500.HandleRequest(this);
            break;
          default:
            response = new StaticHandler(new StringResponse(
              ex.Code,
              "text/plain",
              ex.Message
              )).HandleRequest(this);
            break;
        }
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to process response", this), ex);
        response = Error500.HandleRequest(this);
      }
      SendResponse();
    }

    internal void Close()
    {
      State = HttpStates.CLOSED;

      DebugFormat(
        "{0} - Closing connection after {1} requests", this, requestCount);
      try {
        client.Close();
      }
      catch (Exception) {
      }
      owner.RemoveClient(this);
      if (stream != null) {
        try {
          stream.Dispose();
        }
        catch (ObjectDisposedException) {
        }
      }
    }

    public void Dispose()
    {
      Close();
      if (readStream != null) {
        readStream.Dispose();
      }
    }

    public void Start()
    {
      ReadNext();
    }

    public override string ToString()
    {
      return RemoteEndpoint.ToString();
    }
  }
}
