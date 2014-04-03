using System.IO;
using log4net;

namespace NMaier.SimpleDlna.FileMediaServer
{
  internal sealed class FileReadStream : FileStream
  {
    private const int BUFFER_SIZE = 1 << 16;


    private readonly FileInfo info;

    private readonly static ILog logger = LogManager.GetLogger(typeof(FileReadStream));


    public FileReadStream(FileInfo info)
      : base(info.FullName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete, BUFFER_SIZE, FileOptions.Asynchronous | FileOptions.SequentialScan)
    {
      this.info = info;
      logger.DebugFormat("Opened file {0}", this.info.FullName);
    }


    public override void Close()
    {
      base.Close();
      logger.DebugFormat("Closed file {0}", info.FullName);
    }
  }
}
