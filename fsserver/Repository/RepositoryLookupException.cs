using System;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public sealed class RepositoryLookupException : ArgumentException
  {

    public readonly string Key;



    public RepositoryLookupException(string key)
      : base(String.Format("Failed to lookup {0}", key))
    {
      Key = key;
    }

    public RepositoryLookupException() : base() { }
  }
}
