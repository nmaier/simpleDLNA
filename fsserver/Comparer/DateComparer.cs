using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class DateComparer : TitleComparer
  {

    public override string Description
    {
      get { return "Sort by file date"; }
    }

    public override string Name
    {
      get { return "date"; }
    }




    public override int Compare(IMediaItem x, IMediaItem y)
    {
      var xm = x as IMediaItemMetaData;
      var ym = y as IMediaItemMetaData;
      if (xm != null && ym != null) {
        var rv = xm.ItemDate.CompareTo(ym.ItemDate);
        if (rv != 0) {
          return rv;
        }
      }
      return base.Compare(x, y);
    }
  }
}
