using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NMaier.SimpleDlna.Server.Ssdp
{
  internal sealed class Datagram
  {

    public readonly IPEndPoint EndPoint;
    public readonly string Message;



    public Datagram(IPEndPoint aEndPoint, string aMessage)
    {
      EndPoint = aEndPoint;
      Message = aMessage;
      SendCount = 0;
    }



    public uint SendCount
    {
      get;
      private set;
    }




    public void Send()
    {
      using (var udp = new UdpClient(AddressFamily.InterNetwork)) {
        var msg = Encoding.ASCII.GetBytes(Message);
        udp.Send(msg, msg.Length, EndPoint);
      }
      ++SendCount;
    }
  }
}
