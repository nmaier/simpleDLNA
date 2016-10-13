using System;

namespace NMaier.SimpleDlna.Utilities
{
  public static class SystemInformation
  {
    /// <summary>
    ///   Returns true if applicaton is running under mono
    /// </summary>
    public static bool IsRunningOnMono()
    {
      return Type.GetType("Mono.Runtime") != null;
    }
  }
}
