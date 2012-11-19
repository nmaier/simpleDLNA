using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NMaier.SimpleDlna.Server.Ssdp
{
  internal sealed class Datagram
  {

    public readonly IPEndPoint EndPoint;
    public readonly string Message;
    public readonly bool Sticky;



    public Datagram(IPEndPoint aEndPoint, string aMessage, bool sticky)
    {
      EndPoint = aEndPoint;
      Message = aMessage;
      Sticky = sticky;
      SendCount = 0;
    }



    public uint SendCount
    {
      get;
      private set;
    }




    public void Send(int port)
    {
      using (var udp = new UdpClient(port, AddressFamily.InterNetwork)) {
        var msg = Encoding.ASCII.GetBytes(Message);
        udp.Send(msg, msg.Length, EndPoint);
      }
      ++SendCount;
    }
  }
}
