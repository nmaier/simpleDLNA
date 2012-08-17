using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace NMaier.sdlna.Server
{
  internal class HttpClient : Logging, IRequest, IDisposable
  {

    private string body;
    private uint bodyBytes = 0;
    private readonly byte[] buffer = new byte[BUFFER_SIZE];
    private const int BUFFER_SIZE = 1 << 16;
    private static readonly Regex bytes = new Regex(@"^bytes=(\d+)", RegexOptions.Compiled);
    private readonly TcpClient client;
    private static IHandler Error404 = new StaticHandler(new StringResponse(HttpCodes.NOT_FOUND, "<!doctype html><title>Not found!</title><h1>Not found!</h1><p>The requested resource was not found!</p>"));
    private static IHandler Error416 = new StaticHandler(new StringResponse(HttpCodes.RANGE_NOT_SATISFIABLE, "<!doctype html><title>Requested Range not satisfiable!</title><h1>Requested Range not satisfiable!</h1><p>Nice try, but do not try again :p</p>"));
    private static IHandler Error500 = new StaticHandler(new StringResponse(HttpCodes.INTERNAL_ERROR, "<!doctype html><title>Internal Server Error</title><h1>Internal Server Error</h1><p>Something is very rotten in the State of Denmark!</p>"));
    private bool hasHeaders = false;
    private readonly IHeaders headers = new Headers();
    private DateTime lastActivity;
    private string method;
    private readonly HttpServer owner;
    private string path;
    private MemoryStream readStream = new MemoryStream();
    private IResponse response;
    private Stream responseStream;
    private readonly NetworkStream stream;



    public HttpClient(HttpServer aOwner, TcpClient aClient)
    {
      lastActivity = DateTime.Now;

      owner = aOwner;
      client = aClient;
      stream = client.GetStream();
      client.Client.UseOnlyOverlappedIO = true;
      Read();
    }



    public string Body
    {
      get { return body; }
    }

    public IHeaders Headers
    {
      get { return headers; }
    }

    public bool IsATimeout
    {
      get
      {
        var diff = DateTime.Now - lastActivity;
        return diff.TotalSeconds > 120;
      }
    }

    public IPEndPoint LocalEndPoint
    {
      get { return client.Client.LocalEndPoint as IPEndPoint; }
    }

    public string Method
    {
      get { return method; }
    }

    public string Path
    {
      get { return path; }
    }

    public IPEndPoint RemoteEndpoint
    {
      get { return client.Client.RemoteEndPoint as IPEndPoint; }
    }




    public void Dispose()
    {
      Close();
    }

    public override string ToString()
    {
      return RemoteEndpoint.ToString();
    }

    private void Read()
    {
      try {
        stream.BeginRead(buffer, 0, BUFFER_SIZE, ReadCallback, 0);
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to BeginRead", this), ex);
        Close();
      }
    }

    private void ReadCallback(IAsyncResult result)
    {
      int read = 0;
      try {
        read = stream.EndRead(result);
        if (read < 0) {
          throw new HttpException("Client did not send anything");
        }
        DebugFormat("{0} - Read {1} bytes", this, read);
        readStream.Write(buffer, 0, read);
        lastActivity = DateTime.Now;
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to read data", this), ex);
        Close();
        return;
      }

      try {
        if (!hasHeaders) {
          readStream.Seek(0, SeekOrigin.Begin);
          StreamReader reader = new StreamReader(readStream);
          for (var line = reader.ReadLine(); line != null; line = reader.ReadLine()) {
            line = line.Trim();
            if (line == "") {
              hasHeaders = true;
              readStream = new MemoryStream();
              if (headers.ContainsKey("content-length") && uint.TryParse(headers["content-length"], out bodyBytes)) {
                var bytes = Encoding.ASCII.GetBytes(reader.ReadToEnd());
                readStream.Write(bytes, 0, bytes.Length);
                DebugFormat("Must read body bytes {0}", bodyBytes);
              }
              else {
                readStream = new MemoryStream();
              }
              break;
            }
            if (method == null) {
              var parts = line.Split(new char[] { ' ' }, 3);
              method = parts[0].Trim().ToUpper();
              path = parts[1].Trim();
              InfoFormat("{0} - {1} request for {2}", this, method, path);
            }
            else {
              var parts = line.Split(new char[] { ':' }, 2);
              headers[parts[0]] = Uri.UnescapeDataString(parts[1]).Trim();
            }
          }
        }
        if (bodyBytes != 0 && bodyBytes > readStream.Length) {
          DebugFormat("{0} - Bytes to go {1}", this, bodyBytes - readStream.Length);
          Read();
          return;
        }
        using (readStream) {
          body = Encoding.UTF8.GetString(readStream.ToArray());
          Debug(body);
          Debug(headers);
        }
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to process request", this), ex);
        response = Error500.HandleRequest(this);
        SendResponse();
        return;
      }
      SetupResponse();
    }

    private void SendResponse()
    {
      var body = response.Body;
      var st = response.Status;

      long contentLength = -1;
      string clf;
      if (!response.Headers.TryGetValue("Content-Length", out clf) || !long.TryParse(clf, out contentLength)) {
        try {
          contentLength = body.Length;
          if (contentLength < 0) {
            throw new InvalidDataException();
          }
          response.Headers["Content-Length"] = contentLength.ToString();
        }
        catch (Exception) {
          // pass
        }
      }

      string ar;
      if (st == HttpCodes.OK && contentLength > 0 && headers.TryGetValue("Range", out ar)) {
        try {
          var m = bytes.Match(ar);
          if (!m.Success) {
            throw new Exception("Not parsed!");
          }
          long start = 0;
          if (!long.TryParse(m.Groups[1].Value, out start) || start < 0) {
            throw new Exception("Not parsed");
          }
          if (start >= contentLength) {
            response = Error416.HandleRequest(this);
            SendResponse();
            return;
          }
          if (start > 0) {
            body.Seek(start, SeekOrigin.Begin);
            headers["Content-Range"] = String.Format("bytes {0}-{1}/{2}", start, (contentLength - start - 1), contentLength);
          }
          st = HttpCodes.PARTIAL;
        }
        catch (Exception ex) {
          Warn(String.Format("{0} - Failed to process range request!", this), ex);
        }
      }

      var hb = new StringBuilder();
      hb.AppendFormat("HTTP/1.0 {0} {1}\r\n", (uint)st, HttpPhrases.Phrases[st]);
      hb.Append(response.Headers.HeaderBlock);
      hb.Append("\r\n");

      var rs = new ConcatenatedStream();
      rs.AddStream(new MemoryStream(Encoding.ASCII.GetBytes(hb.ToString())));
      if (method != "HEAD" && body != null) {
        rs.AddStream(body);
      }
      responseStream = rs;
      InfoFormat("{0} - {1} response for {2}", this, (uint)st, path);
      Write();
    }

    private void SetupResponse()
    {
      try {
        var handler = owner.FindHandler(path);
        if (handler == null) {
          throw new Http404Exception();
        }
        response = handler.HandleRequest(this);
        if (response == null) {
          throw new ArgumentNullException();
        }
      }
      catch (Http404Exception ex) {
        Info(String.Format("{0} - Got a 404: {1}", this, this.path), ex);
        response = Error404.HandleRequest(this);
      }
      catch (Exception ex) {
        Warn(String.Format("{0} - Failed to process response", this), ex);
        response = Error500.HandleRequest(this);
      }
      SendResponse();
    }

    private void Write()
    {
      try {
        int bytes = responseStream.Read(buffer, 0, BUFFER_SIZE);
        if (bytes <= 0) {
          DebugFormat("{0} - Done writing response", this);
          Close();
          return;
        }
        stream.BeginWrite(buffer, 0, bytes, WriteCallback, null);
      }
      catch (Exception ex) {
        Debug(String.Format("{0} - Failed to write - Client hung up on me", this), ex);
        Close();
      }
    }

    private void WriteCallback(IAsyncResult result)
    {
      try {
        stream.EndWrite(result);
        lastActivity = DateTime.Now;
      }
      catch (Exception ex) {
        Debug("Failed to write - client hung up on me", ex);
        Close();
        return;
      }

      Write();
    }

    internal void Close()
    {
      try {
        client.Close();
      }
      catch (Exception) { }
      owner.RemoveClient(this);
    }
  }
}
