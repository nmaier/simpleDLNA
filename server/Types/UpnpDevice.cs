using System;
using System.Net;

namespace NMaier.SimpleDlna.Server
{
  internal sealed class UpnpDevice
  {
    public readonly IPAddress Address;

    public readonly Uri Descriptor;

    public readonly string Type;

    public readonly string USN;

    public readonly Guid UUID;

    public UpnpDevice(Guid uuid, string type, Uri descriptor,
      IPAddress address)
    {
      UUID = uuid;
      Type = type;
      Descriptor = descriptor;
      Address = address;

      if (Type.StartsWith("uuid:", StringComparison.Ordinal)) {
        USN = Type;
      }
      else {
        USN = $"uuid:{UUID}::{Type}";
      }
    }
  }
}
