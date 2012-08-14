using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class FileSizeComparer : TitleComparer
  {

    public override string Description
    {
      get { return "Sort by file size"; }
    }

    public override string Name
    {
      get { return "size"; }
    }




    public override int Compare(IMediaItem x, IMediaItem y)
    {
      var xm = x as IMediaItemMetaData;
      var ym = y as IMediaItemMetaData;
      if (xm != null && ym != null) {
        var rv = xm.ItemSize.CompareTo(ym.ItemSize);
        if (rv != 0) {
          return rv;
        }
      }
      return base.Compare(x, y);
    }
  }
}
