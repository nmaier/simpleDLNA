using System;

namespace NMaier.sdlna.FileMediaServer
{
  public sealed class RepositoryLookupException : ArgumentException
  {

    public readonly string Key;



    public RepositoryLookupException(string aKey)
      : base(String.Format("Failed to lookup {0}", aKey))
    {
      Key = aKey;
    }
  }
}
