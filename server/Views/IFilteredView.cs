namespace NMaier.SimpleDlna.Server.Views
{
  public interface IFilteredView : IView
  {
    bool Allowed(IMediaResource item);
  }
}
