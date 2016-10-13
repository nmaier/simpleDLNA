using System.IO;
using log4net;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class FileReadStream : FileStream
  {
    private static readonly ILog logger =
      LogManager.GetLogger(typeof (FileReadStream));

    private readonly FileInfo info;

    private bool killed;

    public FileReadStream(FileInfo info)
      : base(info.FullName, FileMode.Open,
             FileAccess.Read, FileShare.ReadWrite | FileShare.Delete,
             1,
             FileOptions.Asynchronous | FileOptions.SequentialScan)
    {
      this.info = info;
      logger.DebugFormat("Opened file {0}", this.info.FullName);
    }

    public void Kill()
    {
      logger.DebugFormat("Killed file {0}", info.FullName);
      killed = true;
      Close();
      Dispose();
    }

    public override void Close()
    {
      if (!killed) {
        FileStreamCache.Recycle(this);
        return;
      }
      base.Close();
      logger.DebugFormat("Closed file {0}", info.FullName);
    }

    protected override void Dispose(bool disposing)
    {
      if (!killed) {
        return;
      }
      base.Dispose(disposing);
    }
  }
}
