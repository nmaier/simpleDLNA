using System;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.GUI
{
  [Serializable]
  public sealed class ServerDescription
  {
    public ServerDescription()
    {
      UserAgents = Ips = Macs = Views = Directories = new string[0];
    }

    public bool Active { get; set; }

    public string[] Directories { get; set; }

    public string[] Ips { get; set; }

    public string[] Macs { get; set; }

    public string Name { get; set; }

    public string Order { get; set; }

    public bool OrderDescending { get; set; }

    public DlnaMediaTypes Types { get; set; }

    public string[] UserAgents { get; set; }

    public string[] Views { get; set; }

    public void AdoptInfo(ServerDescription other)
    {
      if (other == null) {
        throw new ArgumentNullException(nameof(other));
      }
      Directories = other.Directories;
      Name = other.Name;
      Order = other.Order;
      OrderDescending = other.OrderDescending;
      Types = other.Types;
      Views = other.Views;
      Macs = other.Macs;
      Ips = other.Ips;
      UserAgents = other.UserAgents;
    }

    public void ToggleActive()
    {
      Active = !Active;
    }
  }
}
