using System;
using System.Collections.Generic;
using System.IO;

namespace NMaier.sdlna.Util
{
  public class ConcatenatedStream : Stream
  {

    Queue<Stream> streams = new Queue<Stream>();



    public override bool CanRead
    {
      get { return true; }
    }

    public override bool CanSeek
    {
      get { return false; }
    }

    public override bool CanWrite
    {
      get { return false; }
    }

    public override long Length
    {
      get { throw new NotImplementedException(); }
    }

    public override long Position
    {
      get
      {
        throw new NotImplementedException();
      }
      set
      {
        throw new NotImplementedException();
      }
    }




    public void AddStream(Stream s)
    {
      streams.Enqueue(s);
    }

    public override void Flush()
    {
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
      if (streams.Count == 0) {
        return 0;
      }

      int read = streams.Peek().Read(buffer, offset, count);
      if (read < count) {
        streams.Dequeue().Dispose();
        return read + Read(buffer, offset + read, count - read);
      }
      return read;
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
      throw new NotImplementedException();
    }

    public override void SetLength(long value)
    {
      throw new NotImplementedException();
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }
  }
}
