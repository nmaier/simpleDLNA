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


    public void Send()
    {
      var msg = Encoding.ASCII.GetBytes(Message);
      foreach (var external in IP.ExternalIPAddresses) {
        try {
          var client = new UdpClient(new IPEndPoint(external, 0));
          client.BeginSend(msg, msg.Length, EndPoint, result =>
          {
            try {
              client.EndSend(result);
            }
            catch (Exception ex) {
              Error(ex);
            }
            finally {
              try {
                client.Close();
              }
              catch (Exception) {
              }
            }
          }, null);
        }
        catch (Exception ex) {
          Error(ex);
        }
      }
      ++SendCount;
    }
  }
}
