using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using NMaier.SimpleDlna.Server;
using System.IO.Pipes;
using System.Threading;

namespace NMaier.SimpleDlna.GUI
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      using (Mutex mutex = new Mutex(false, @"Global\simpledlnaguilock")) {
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
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        Application.Run(new FormMain());
      }
    }
  }
}
