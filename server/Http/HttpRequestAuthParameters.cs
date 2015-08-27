using NMaier.SimpleDlna.Utilities;
using System.Net;

namespace NMaier.SimpleDlna.Server.Http
{
  public class HttpRequestAuthParameters
  {
    string _userAgent;
    public string UserAgent { get { return _userAgent; } }
    IPAddress _address;
    public IPAddress Address { get { return _address; } }
    string _mac;
    public string Mac { get { return _mac; } }

    public HttpRequestAuthParameters(IHeaders headers, IPEndPoint endPoint) {
      if (headers != null) {
        headers.TryGetValue("User-Agent", out _userAgent);
      }
      if (endPoint != null) {
        _address = endPoint.Address;
        _mac = IP.GetMAC(_address);
      }
    }

    public override string ToString()
    {
      return string.Format("[{0}][{1}][{2}]",_address,_mac,_userAgent);
    }
  }
}
