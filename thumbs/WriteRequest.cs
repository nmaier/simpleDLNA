using System;
using System.Diagnostics;
using System.IO;

namespace NMaier.sdlna.Thumbnails
{
  internal class WriteRequest : IDisposable
  {

    public byte[] buffer;
    public Stream InStream;
    public Stream OutStream;
    public Process Request;
    private IAsyncResult res;



    public WriteRequest(Process aProcess, Stream aInStream, Stream aOutStream)
    {
      Request = aProcess;
      InStream = aInStream;
      OutStream = aOutStream;
      buffer = new byte[1 << 17];
    }




    public void Dispose()
    {
      buffer = null;
    }

    public void Write()
    {
      res = null;
      if (Request.HasExited) {
        return;
      }
      var read = InStream.Read(buffer, 0, buffer.Length);
      if (read != 0) {
        try {
          res = OutStream.BeginWrite(buffer, 0, buffer.Length, WriteCallback, null);
        }
        catch (Exception) {
          // hung up, probably
        }
      }
    }

    private void WriteCallback(IAsyncResult ar)
    {
      try {
        OutStream.EndWrite(ar);
        Write();
      }
      catch (Exception) {
        // hung up, probably
      }
    }
  }
}
