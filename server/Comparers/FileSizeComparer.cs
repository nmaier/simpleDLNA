using NMaier.SimpleDlna.Server.Metadata;

namespace NMaier.SimpleDlna.Server.Comparers
{
  internal class FileSizeComparer : TitleComparer
  {
    public override string Description
    {
      get
      {
        return "Sort by file size";
      }
    }
    public override string Name
    {
      get
      {
        return "size";
      }
    }


    public override int Compare(IMediaItem x, IMediaItem y)
    {
      var xm = x as IMetaInfo;
      var ym = y as IMetaInfo;
      if (xm != null && ym != null && xm.InfoSize.HasValue && ym.InfoSize.HasValue) {
        var rv = xm.InfoSize.Value.CompareTo(ym.InfoSize.Value);
        if (rv != 0) {
          return rv;
        }
      }
      return base.Compare(x, y);
    }
  }
}
