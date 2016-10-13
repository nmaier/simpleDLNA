using System;
using System.Runtime.Serialization;

namespace NMaier.SimpleDlna.Utilities
{
  [Serializable]
  public sealed class RepositoryLookupException : ArgumentException
  {
    private RepositoryLookupException(SerializationInfo info,
      StreamingContext context)
      : base(info, context)
    {
    }

    public RepositoryLookupException()
    {
    }

    public RepositoryLookupException(string key)
      : base($"Failed to lookup {key}")
    {
      Key = key;
    }

    public RepositoryLookupException(string message, Exception inner)
      : base(message, inner)
    {
    }

    public string Key { get; private set; }
  }
}
