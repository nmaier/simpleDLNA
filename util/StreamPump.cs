using System;
using System.IO;
using System.Threading;
using log4net;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class StreamPump : IDisposable
  {
    private readonly byte[] buffer;

    private readonly SemaphoreSlim sem = new SemaphoreSlim(0, 1);

    public StreamPump(Stream inputStream, Stream outputStream, int bufferSize)
    {
      buffer = new byte[bufferSize];
      Input = inputStream;
      Output = outputStream;
    }

    public Stream Input { get; }

    public Stream Output { get; }

    public void Dispose()
    {
      sem.Dispose();
    }

    private void Finish(StreamPumpResult result, StreamPumpCallback callback)
    {
      callback?.BeginInvoke(this, result, callback.EndInvoke, null);
      try {
        sem.Release();
      }
      catch (ObjectDisposedException) {
        // ignore
      }
      catch (Exception ex) {
        LogManager.GetLogger(typeof (StreamPump)).Error(ex.Message, ex);
      }
    }

    public void Pump(StreamPumpCallback callback)
    {
      try {
        Input.BeginRead(buffer, 0, buffer.Length, readResult =>
        {
          try {
            var read = Input.EndRead(readResult);
            if (read <= 0) {
              Finish(StreamPumpResult.Delivered, callback);
              return;
            }

            try {
              Output.BeginWrite(buffer, 0, read, writeResult =>
              {
                try {
                  Output.EndWrite(writeResult);
                  Pump(callback);
                }
                catch (Exception) {
                  Finish(StreamPumpResult.Aborted, callback);
                }
              }, null);
            }
            catch (Exception) {
              Finish(StreamPumpResult.Aborted, callback);
            }
          }
          catch (Exception) {
            Finish(StreamPumpResult.Aborted, callback);
          }
        }, null);
      }
      catch (Exception) {
        Finish(StreamPumpResult.Aborted, callback);
      }
    }

    public bool Wait(int timeout)
    {
      return sem.Wait(timeout);
    }
  }
}
