using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using NMaier.SimpleDlna.Utilities;

namespace NMaier.SimpleDlna.Server.Ssdp
{
  internal sealed class Datagram : Logging
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
      var msg = Encoding.ASCII.GetBytes(Message);
      foreach (var external in IP.ExternalAddresses) {
        try {
          var client = new UdpClient(new IPEndPoint(external, port));
          client.BeginSend(msg, msg.Length, EndPoint, SendCallback, client);
        }
        catch (Exception ex) {
          Error(ex);
        }
      }
      ++SendCount;
    }

    private void SendCallback(IAsyncResult result)
    {
      using (var client = result.AsyncState as UdpClient) {
        try {
          client.EndSend(result);
        }
        catch (Exception ex) {
          Error(ex);
        }
      }
    }
  }
}
