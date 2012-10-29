using System;
using System.Diagnostics;
using System.IO;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class WriteRequest : IDisposable
  {

    private byte[] buffer;
    private Stream inStream;
    private Stream outStream;
    private Process request;



    public WriteRequest(Process process, Stream inStream, Stream outStream)
    {
      this.request = process;
      this.inStream = inStream;
      this.outStream = outStream;
      buffer = new byte[1 << 17];
    }



    public Stream InStream { get { return inStream; } }

    public Stream OutStream { get { return outStream; } }

    public Process Request { get { return request; } }




    public void Dispose()
    {
      buffer = null;
    }

    public void Write()
    {
      if (request.HasExited) {
        return;
      }
      var read = inStream.Read(buffer, 0, buffer.Length);
      if (read != 0) {
        try {
          outStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, null);
        }
        catch (IOException) {
          // hung up, probably
        }
      }
    }

    private void WriteCallback(IAsyncResult ar)
    {
      try {
        outStream.EndWrite(ar);
        Write();
      }
      catch (IOException) {
        // hung up, probably
      }
    }
  }
}
