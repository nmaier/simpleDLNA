using NMaier.sdlna.Server;
using NMaier.sdlna.Server.Metadata;

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
      var xm = x as IMetaInfo;
      var ym = y as IMetaInfo;
      if (xm != null && ym != null && xm.Size.HasValue && ym.Size.HasValue) {
        var rv = xm.Size.Value.CompareTo(ym.Size.Value);
        if (rv != 0) {
          return rv;
        }
      }
      return base.Compare(x, y);
    }
  }
}
