namespace NMaier.SimpleDlna.Thumbnails
{
  internal sealed class Thumbnail : IThumbnail
  {
    private readonly byte[] data;

    internal Thumbnail(int width, int height, byte[] data)
    {
      Width = width;
      Height = height;
      this.data = data;
    }

    public int Height { get; }

    public int Width { get; }

    public byte[] GetData()
    {
      return data;
    }
  }
}
