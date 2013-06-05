using System;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.GUI
{
  [Serializable]
  public class ServerDescription
  {
    public string[] Directories { get; set; }
    public string Name { get; set; }
    public string Order { get; set; }
    public bool OrderDescending { get; set; }
    public DlnaMediaTypes Types { get; set; }
    public string[] Views { get; set; }
    public bool Active { get; set; }

    public void AdoptInfo(ServerDescription other)
    {
      Directories = other.Directories;
      Name = other.Name;
      Order = other.Order;
      OrderDescending = other.OrderDescending;
      Types = other.Types;
      Views = other.Views;
    }
  }
}
