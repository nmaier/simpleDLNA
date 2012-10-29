
namespace NMaier.SimpleDlna.Utilities
{
  public static class Formatting
  {


    public static string FormatFileSize(long size)
    {
      if (size < 900) {
        return string.Format("{0} B", size);
      }
      double ds = size / 1024.0;
      if (ds < 900) {
        return string.Format("{0:F2} KB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F2} MB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F3} GB", ds);
      }
      ds /= 1024.0;
      if (ds < 900) {
        return string.Format("{0:F3} TB", ds);
      }
      ds /= 1024.0;
      return string.Format("{0:F4} PB", ds);
    }
  }
}
