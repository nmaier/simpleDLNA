using System;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

namespace NMaier.SimpleDlna.GUI
{
  internal static class Program
  {
    [STAThread]
    private static void Main()
    {
      using (var mutex = new Mutex(false, @"Global\simpledlnaguilock")) {
#if !DEBUG
        if (!mutex.WaitOne(0, false)) {
          using (var pipe = new NamedPipeClientStream(
              ".", "simpledlnagui", PipeDirection.Out)) {
            try {
              pipe.Connect(10000);
              pipe.WriteByte(1);
            }
            catch (Exception) {
            }
            return;
          }
        }
        GC.Collect();
#endif

        using (var main = new FormMain()) {
          try {
            Application.Run(main);
          }
          catch (Exception ex) {
            log4net.LogManager.GetLogger(typeof(Program)).Fatal(
              "Encountered fatal unhandled exception", ex);
            MessageBox.Show(
              string.Format(
                "Encountered an unhandled error. Will exit now.\n\n{0}\n{1}",
                ex.Message, ex.StackTrace),
              "Error",
              MessageBoxButtons.OK,
              MessageBoxIcon.Error
            );
            throw;
          }
        }
      }
    }
  }
}
