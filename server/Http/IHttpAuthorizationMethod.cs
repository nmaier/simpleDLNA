namespace NMaier.SimpleDlna.Server.Http
{
  public interface IHttpAuthorizationMethod
  {
    /// <summary>
    /// Checks if a request is authorized.
    /// </summary>
    /// <param name="headers">Client supplied HttpHeaders.</param>
    /// <param name="endPoint">Client EndPoint</param>
    /// <param name="mac">Client MAC address</param>
    /// <returns>true if authorized</returns>
    bool Authorize(HttpRequestAuthParameters ap);//IHeaders headers, IPEndPoint endPoint, string mac);
  }
}
