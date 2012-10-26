using System;
using System.Diagnostics;
using System.IO;

namespace NMaier.sdlna.Util
{
  public sealed class ReadRequest : IDisposable
  {

    public byte[] buffer;
    public Stream InStream;
    public MemoryStream OutStream;
    public Process Request;
    private IAsyncResult res;



    public ReadRequest(Process aProcess, Stream aInStream)
    {
      Request = aProcess;
      InStream = aInStream;
      OutStream = new MemoryStream();
      buffer = new byte[1 << 17];
    }




    public void Dispose()
    {
      OutStream.Dispose();
      buffer = null;
    }

    public void Finish()
    {
      if (res != null && !res.IsCompleted && !res.AsyncWaitHandle.WaitOne(5000)) {
        throw new ArgumentException("Buffer deadlocked!");
      }
      for (var read = InStream.Read(buffer, 0, buffer.Length); read != 0; read = InStream.Read(buffer, 0, buffer.Length)) {
        OutStream.Write(buffer, 0, read);
      }
      OutStream.Seek(0, SeekOrigin.Begin);
    }

    public void Read()
    {
      res = InStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
    }

    private void ReadCallback(IAsyncResult ar)
    {
      var read = InStream.EndRead(ar);
      if (read != 0) {
        OutStream.Write(buffer, 0, read);
        if (!Request.HasExited) {
          Read();
          return;
        }
      }
      res = null;
    }
  }
}
