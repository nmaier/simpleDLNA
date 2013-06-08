using System;
using System.IO.Pipes;
using System.Threading;
using System.Windows.Forms;

[assembly: CLSCompliant(true)]
namespace NMaier.SimpleDlna.GUI
{
  static class Program
  {
    [STAThread]
    static void Main()
    {
      using (Mutex mutex = new Mutex(false, @"Global\simpledlnaguilock")) {
#if !DEBUG
        if (!mutex.WaitOne(0, false)) {
          using (var pipe = new NamedPipeClientStream(".", "simpledlnagui", PipeDirection.Out)) {
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

        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
      }
    }
  }
}
