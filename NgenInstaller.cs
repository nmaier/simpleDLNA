using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration.Install;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace NMaier.SimpleDlna
{
  [RunInstaller(true)]
  public class NgenInstaller : Installer
  {
    public override void Install(IDictionary stateSaver)
    {
      base.Install(stateSaver);
      using (var proc = new Process()) {
        proc.StartInfo.FileName = Path.Combine(
          RuntimeEnvironment.GetRuntimeDirectory(),
          "ngen.exe"
          );
        proc.StartInfo.Arguments = $"install /nologo \"{Assembly.GetExecutingAssembly().Location}\"";
        proc.StartInfo.UseShellExecute = false;
        proc.StartInfo.CreateNoWindow = true;
        proc.Start();
        proc.WaitForExit();
        if (proc.ExitCode != 0) {
          throw new Exception("Failed to run ngen");
        }
      }
    }
  }
}
