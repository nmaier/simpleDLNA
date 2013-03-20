using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.FileMediaServer.Comparers
{
  internal class DateComparer : TitleComparer
  {
    public override string Description
    {
      get
      {
        return "Sort by file date";
      }
    }
    public override string Name
    {
      get
      {
        return "date";
      }
    }


    public override int Compare(IMediaItem x, IMediaItem y)
    {
      var xm = x as IMetaInfo;
      var ym = y as IMetaInfo;
      if (xm != null && ym != null) {
        var rv = xm.InfoDate.CompareTo(ym.InfoDate);
        if (rv != 0) {
          return rv;
        }
      }
      return base.Compare(x, y);
    }
  }
}
