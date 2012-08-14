using NMaier.sdlna.Server;

namespace NMaier.sdlna.FileMediaServer
{
  class TitleComparer : IItemComparer
  {

    public virtual string Description
    {
      get { return "Sort alphabetically"; }
    }

    public virtual string Name
    {
      get { return "title"; }
    }




    public virtual int Compare(IMediaItem x, IMediaItem y)
    {
      return x.Title.ToLower().CompareTo(y.Title.ToLower());
    }
  }
}