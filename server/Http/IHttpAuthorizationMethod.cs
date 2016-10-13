using System.Net;

namespace NMaier.SimpleDlna.Server
{
  public interface IHttpAuthorizationMethod
  {
    /// <summary>
    ///   Checks if a request is authorized.
    /// </summary>
    /// <param name="headers">Client supplied HttpHeaders.</param>
    /// <param name="endPoint">Client EndPoint</param>
    /// <param name="mac">Client MAC address</param>
    /// <returns>true if authorized</returns>
    bool Authorize(IHeaders headers, IPEndPoint endPoint, string mac);
  }
}
