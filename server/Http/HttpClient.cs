using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class HttpClient : Logging, IRequest, IDisposable
  {
    private const uint BEGIN_TIMEOUT = 30;

    private const int BUFFER_SIZE = 1 << 16;

    private const string CRLF = "\r\n";

    private static readonly Regex bytes =
      new Regex(@"^bytes=(\d+)(?:-(\d+)?)?$", RegexOptions.Compiled);

    private static readonly IHandler error403 =
      new StaticHandler(new StringResponse(
                          HttpCode.Denied,
                          "<!doctype html><title>Access denied!</title><h1>Access denied!</h1><p>You're not allowed to access the requested resource.</p>"
                          )
        );

    private static readonly IHandler error404 =
      new StaticHandler(new StringResponse(
                          HttpCode.NotFound,
                          "<!doctype html><title>Not found!</title><h1>Not found!</h1><p>The requested resource was not found!</p>"
                          )
        );

    private static readonly IHandler error416 =
      new StaticHandler(new StringResponse(
                          HttpCode.RangeNotSatisfiable,
                          "<!doctype html><title>Requested Range not satisfiable!</title><h1>Requested Range not satisfiable!</h1><p>Nice try, but do not try again :p</p>"
                          )
        );

    private static readonly IHandler error500 =
      new StaticHandler(new StringResponse(
                          HttpCode.InternalError,
                          "<!doctype html><title>Internal Server Error</title><h1>Internal Server Error</h1><p>Something is very rotten in the State of Denmark!</p>"
                          )
        );

    private readonly byte[] buffer = new byte[2048];

    private readonly TcpClient client;

    private readonly HttpServer owner;

    private readonly uint readTimeout =
      (uint)TimeSpan.FromMinutes(1).TotalSeconds;

    private readonly NetworkStream stream;

    private readonly uint writeTimeout =
      (uint)TimeSpan.FromMinutes(180).TotalSeconds;

    private uint bodyBytes;

    private bool hasHeaders;

    private DateTime lastActivity;

    private MemoryStream readStream;

    private uint requestCount;

    private IResponse response;

    private HttpStates state;

    public HttpClient(HttpServer aOwner, TcpClient aClient)
    {
      State = HttpStates.Accepted;
      lastActivity = DateTime.Now;

      owner = aOwner;
      client = aClient;
      stream = client.GetStream();
      client.Client.UseOnlyOverlappedIO = true;

      RemoteEndpoint = client.Client.RemoteEndPoint as IPEndPoint;
      LocalEndPoint = client.Client.LocalEndPoint as IPEndPoint;
    }

    private HttpStates State
    {
      set {
        lastActivity = DateTime.Now;
        state = value;
      }
    }

    public bool IsATimeout
    {
      get {
        var diff = (DateTime.Now - lastActivity).TotalSeconds;
        switch (state) {
        case HttpStates.Accepted:
        case HttpStates.ReadBegin:
        case HttpStates.WriteBegin:
          return diff > BEGIN_TIMEOUT;
        case HttpStates.Reading:
          return diff > readTimeout;
        case HttpStates.Writing:
          return diff > writeTimeout;
        case HttpStates.Closed:
          return true;
        default:
          throw new InvalidOperationException("Invalid state");
        }
      }
    }

    public void Dispose()
    {
      Close();
      readStream?.Dispose();
    }

    public string Body { get; private set; }

    public IHeaders Headers { get; } = new Headers();

    public IPEndPoint LocalEndPoint { get; }

    public string Method { get; private set; }

    public string Path { get; private set; }

    public IPEndPoint RemoteEndpoint { get; }

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
        // ignored
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
            !Headers.TryGetValue("Range", out ar)) {
          return responseBody;
        }
        var m = bytes.Match(ar);
        if (!m.Success) {
          throw new InvalidDataException("Not parsed!");
        }
        var totalLength = contentLength;
        long start;
        long end;
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
          rangedResponse = error416.HandleRequest(this);
          return rangedResponse.Body;
        }

        if (start > 0) {
          responseBody.Seek(start, SeekOrigin.Current);
        }
        contentLength = end - start + 1;
        rangedResponse.Headers["Content-Length"] = contentLength.ToString();
        rangedResponse.Headers.Add(
          "Content-Range",
          $"bytes {start}-{end}/{totalLength}"
          );
        status = HttpCode.Partial;
      }
      catch (Exception ex) {
        Warn($"{this} - Failed to process range request!", ex);
      }
      return responseBody;
    }

    private void Read()
    {
      try {
        stream.BeginRead(buffer, 0, buffer.Length, ReadCallback, 0);
      }
      catch (IOException ex) {
        Warn($"{this} - Failed to BeginRead", ex);
        Close();
      }
    }

    private void ReadCallback(IAsyncResult result)
    {
      if (state == HttpStates.Closed) {
        return;
      }

      State = HttpStates.Reading;

      try {
        var read = stream.EndRead(result);
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
          for (var line = reader.ReadLine();
            line != null;
            line = reader.ReadLine()) {
            line = line.Trim();
            if (string.IsNullOrEmpty(line)) {
              hasHeaders = true;
              readStream = StreamManager.GetStream();
              if (Headers.ContainsKey("content-length") &&
                  uint.TryParse(Headers["content-length"], out bodyBytes)) {
                if (bodyBytes > 1 << 20) {
                  throw new IOException("Body too long");
                }
                var ascii = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                readStream.Write(ascii, 0, ascii.Length);
                DebugFormat("Must read body bytes {0}", bodyBytes);
              }
              break;
            }
            if (Method == null) {
              var parts = line.Split(new[] {' '}, 3);
              Method = parts[0].Trim().ToUpperInvariant();
              Path = parts[1].Trim();
              DebugFormat("{0} - {1} request for {2}", this, Method, Path);
            }
            else {
              var parts = line.Split(new[] {':'}, 2);
              Headers[parts[0]] = Uri.UnescapeDataString(parts[1]).Trim();
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
          Body = Encoding.UTF8.GetString(readStream.ToArray());
          Debug(Body);
          Debug(Headers);
        }
        SetupResponse();
      }
      catch (Exception ex) {
        Warn($"{this} - Failed to process request", ex);
        response = error500.HandleRequest(this);
        SendResponse();
      }
    }

    private void ReadNext()
    {
      Method = null;
      Headers.Clear();
      hasHeaders = false;
      Body = null;
      bodyBytes = 0;
      readStream = StreamManager.GetStream();

      ++requestCount;
      State = HttpStates.ReadBegin;

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
        if (Method != "HEAD" && responseBody != null) {
          responseStream.AddStream(responseBody);
          responseBody = null;
        }
        InfoFormat("{0} - {1} response for {2}", this, (uint)statusCode, Path);
        state = HttpStates.Writing;
        var sp = new StreamPump(responseStream, stream, BUFFER_SIZE);
        sp.Pump((pump, result) =>
        {
          pump.Input.Close();
          pump.Input.Dispose();
          if (result == StreamPumpResult.Delivered) {
            DebugFormat("{0} - Done writing response", this);

            string conn;
            if (Headers.TryGetValue("connection", out conn) &&
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
        responseBody?.Dispose();
      }
    }

    private void SetupResponse()
    {
      State = HttpStates.WriteBegin;
      try {
        if (!owner.AuthorizeClient(this)) {
          throw new HttpStatusException(HttpCode.Denied);
        }
        if (string.IsNullOrEmpty(Path)) {
          throw new HttpStatusException(HttpCode.NotFound);
        }
        var handler = owner.FindHandler(Path);
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
        Warn(String.Format("{0} - Got a {2}: {1}", this, Path, ex.Code), ex);
#else
        InfoFormat("{0} - Got a {2}: {1}", this, Path, ex.Code);
#endif
        switch (ex.Code) {
        case HttpCode.NotFound:
          response = error404.HandleRequest(this);
          break;
        case HttpCode.Denied:
          response = error403.HandleRequest(this);
          break;
        case HttpCode.InternalError:
          response = error500.HandleRequest(this);
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
        Warn($"{this} - Failed to process response", ex);
        response = error500.HandleRequest(this);
      }
      SendResponse();
    }

    internal void Close()
    {
      State = HttpStates.Closed;

      DebugFormat(
        "{0} - Closing connection after {1} requests", this, requestCount);
      try {
        client.Close();
      }
      catch (Exception) {
        // ignored
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

    public void Start()
    {
      ReadNext();
    }

    public override string ToString()
    {
      return RemoteEndpoint.ToString();
    }

    internal enum HttpStates
    {
      Accepted,
      Closed,
      ReadBegin,
      Reading,
      WriteBegin,
      Writing
    }
  }
}
