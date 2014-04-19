namespace NMaier.SimpleDlna.Thumbnails
{
  internal sealed class Thumbnail : IThumbnail
  {
    private readonly byte[] data;

    private readonly int height;

    private readonly int width;

    internal Thumbnail(int width, int height, byte[] data)
    {
      this.width = width;
      this.height = height;
      this.data = data;
    }

    public int Height
    {
      get
      {
        return height;
      }
    }

    public int Width
    {
      get
      {
        return width;
      }
    }

    public byte[] GetData()
    {
      return data;
    }
  }
}
