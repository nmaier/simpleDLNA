using System;
using System.Collections.Generic;
using System.IO;

namespace NMaier.SimpleDlna.Utilities
{
  public sealed class ConcatenatedStream : Stream
  {
    private readonly Queue<Stream> streams = new Queue<Stream>();


    public override bool CanRead
    {
      get
      {
        return true;
      }
    }
    public override bool CanSeek
    {
      get
      {
        return false;
      }
    }
    public override bool CanWrite
    {
      get
      {
        return false;
      }
    }
    public override long Length
    {
      get
      {
        throw new NotSupportedException();
      }
    }
    public override long Position
    {
      get
      {
        throw new NotSupportedException();
      }
      set
      {
        throw new NotSupportedException();
      }
    }


    public void AddStream(Stream stream)
    {
      streams.Enqueue(stream);
    }

    public override void Flush()
    {
      return;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (streams.Count == 0) {
        return 0;
      }

      var read = streams.Peek().Read(buffer, offset, count);
      if (read < count) {
        streams.Dequeue().Dispose();
        return read + Read(buffer, offset + read, count - read);
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
