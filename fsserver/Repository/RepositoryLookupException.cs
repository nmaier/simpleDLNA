using System;

namespace NMaier.SimpleDlna.FileMediaServer
{
  public sealed class RepositoryLookupException : ArgumentException
  {
    public readonly string Key;


    public RepositoryLookupException()
    {
    }
    public RepositoryLookupException(string key)
      : base(String.Format("Failed to lookup {0}", key))
    {
      Key = key;
    }
  }
}
