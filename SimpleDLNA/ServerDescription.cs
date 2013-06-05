using System;
using System.Collections.Generic;
using NMaier.SimpleDlna.Server;

namespace NMaier.SimpleDlna.GUI
{
  [Serializable]
  public class ServerDescription
  {
    protected ServerDescription()
    {
    }


    public ServerDescription(string name, string order, bool orderDescending, DlnaMediaTypes types, List<string> views, List<string> directories)
    {
      Name = name;
      Order = order;
      OrderDescending = orderDescending;
      Types = types;
      Views = views.ToArray();
      Directories = directories.ToArray();
    }


    public bool Active { get; set; }
    public string[] Directories { get; set; }
    public string Name { get; set; }
    public string Order { get; set; }
    public bool OrderDescending { get; set; }
    public DlnaMediaTypes Types { get; set; }
    public string[] Views { get; set; }


    public void AdoptInfo(ServerDescription other)
    {
      Directories = other.Directories;
      Name = other.Name;
      Order = other.Order;
      OrderDescending = other.OrderDescending;
      Types = other.Types;
      Views = other.Views;
    }

    public void ToggleActive()
    {
      Active = !Active;
    }
  }
}
