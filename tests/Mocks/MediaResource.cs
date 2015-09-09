using NMaier.SimpleDlna.Server;
using System;
using System.IO;

namespace tests.Mocks
{
  public class MediaResource : IMediaResource
  {
    public IMediaCoverResource Cover { get; set; }

    public string Id { get; set; }
 
    public DlnaMediaTypes MediaType { get; set; }

    public string Path { get; set; }

    public string PN { get; set; }

    public IHeaders Properties { get; set; }

    public string Title { get; set; }

    public DlnaMime Type { get; set; }

    public int CompareTo(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public Stream CreateContentStream()
    {
      throw new NotImplementedException();
    }

    public bool Equals(IMediaItem other)
    {
      throw new NotImplementedException();
    }

    public string ToComparableTitle()
    {
      return this.Title;
    }
  }
}
