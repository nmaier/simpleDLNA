using System;
using System.Diagnostics;
using System.IO;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class ReadRequest : IDisposable
  {

    private byte[] buffer;
    private Stream inStream;
    private MemoryStream outStream;
    private Process process;
    private IAsyncResult res;



    public ReadRequest(Process process, Stream inStream)
    {
      this.process = process;
      this.inStream = inStream;
      outStream = new MemoryStream();
      buffer = new byte[1 << 17];
    }



    public Stream InStream { get { return inStream; } }

    public Stream OutStream { get { return outStream; } }

    public Process Process { get { return process; } }




    public void Dispose()
    {
      outStream.Dispose();
      buffer = null;
    }

    public void Finish()
    {
      if (res != null && !res.IsCompleted && !res.AsyncWaitHandle.WaitOne(5000)) {
        throw new ArgumentException("Buffer deadlocked!");
      }
      for (var read = inStream.Read(buffer, 0, buffer.Length); read != 0; read = inStream.Read(buffer, 0, buffer.Length)) {
        outStream.Write(buffer, 0, read);
      }
      outStream.Seek(0, SeekOrigin.Begin);
    }

    public void Read()
    {
      res = inStream.BeginRead(buffer, 0, buffer.Length, ReadCallback, null);
    }

    private void ReadCallback(IAsyncResult ar)
    {
      var read = inStream.EndRead(ar);
      if (read != 0) {
        outStream.Write(buffer, 0, read);
        if (!process.HasExited) {
          Read();
          return;
        }
      }
      res = null;
    }
  }
}
