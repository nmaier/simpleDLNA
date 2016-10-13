using System;
using System.Collections.Generic;
using System.IO;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class ConcatenatedStream : Stream
  {
    private readonly Queue<Stream> streams = new Queue<Stream>();

    public override bool CanRead => true;

    public override bool CanSeek => false;

    public override bool CanWrite => false;

    public override long Length
    {
      get { throw new NotSupportedException(); }
    }

    public override long Position
    {
      get { throw new NotSupportedException(); }
      set { throw new NotSupportedException(); }
    }

    public void AddStream(Stream stream)
    {
      streams.Enqueue(stream);
    }

    public override void Close()
    {
      foreach (var stream in streams) {
        stream.Close();
        stream.Dispose();
      }
      streams.Clear();
      base.Close();
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (streams.Count == 0) {
        return 0;
      }

      var read = streams.Peek().Read(buffer, offset, count);
      if (read < count) {
        var sndRead = streams.Peek().Read(buffer, offset + read, count - read);
        if (sndRead <= 0) {
          streams.Dequeue().Dispose();
          return read + Read(buffer, offset + read, count - read);
        }
        read += sndRead;
      }
      return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotSupportedException();
    }

    public override void SetLength(long value)
    {
      throw new NotSupportedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotSupportedException();
    }
  }
}
