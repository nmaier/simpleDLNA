using System;
using System.IO;
using System.Threading;

namespace NMaier.SimpleDlna.Utilities
{
  public enum StreamPumpResult
  {
    Delivered,
    Aborted
  };

  public delegate void StreamPumpCallback(StreamPump pump, StreamPumpResult result);

  public class StreamPump : Logging
  {

    byte[] buffer;
    private StreamPumpCallback callback;
    public readonly Stream Input, Output;
    SemaphoreSlim sem = new SemaphoreSlim(0, 1);



    public StreamPump(Stream inputStream, Stream outputStream, StreamPumpCallback callback, int bufferSize)
    {
      buffer = new byte[bufferSize];
      Input = inputStream;
      Output = outputStream;
      this.callback = callback;
      Pump();
    }




    public bool Wait(int timeout)
    {
      return sem.Wait(timeout);
    }

    private void Finish(StreamPumpResult result)
    {
      if (callback != null) {
        callback.BeginInvoke(this, result, ir => { callback.EndInvoke(ir); }, null);
      }
      sem.Release();
    }

    private void Pump()
    {
      try {
        Input.BeginRead(buffer, 0, buffer.Length, readResult =>
        {
          try {
            var read = Input.EndRead(readResult);
            if (read <= 0) {
              Finish(StreamPumpResult.Delivered);
              return;
            }

            try {
              Output.BeginWrite(buffer, 0, read, writeResult =>
              {
                try {
                  Output.EndWrite(writeResult);
                  Pump();
                }
                catch (Exception ex) {
                  Debug(ex);
                  Finish(StreamPumpResult.Aborted);
                }
              }, null);
            }
            catch (Exception ex) {
              Debug(ex);
              Finish(StreamPumpResult.Aborted);
            }
          }
          catch (Exception ex) {
            Error(ex);
            Finish(StreamPumpResult.Aborted);
          }
        }, null);
      }
      catch (Exception ex) {
        Error(ex);
        Finish(StreamPumpResult.Aborted);
      }
    }
  }
}
