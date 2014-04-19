namespace NMaier.SimpleDlna.Thumbnails
{
  public interface IThumbnail
  {
    int Height { get; }

    int Width { get; }

    byte[] GetData();
  }
}
