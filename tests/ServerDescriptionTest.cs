using NUnit.Framework;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.GUI;
using NMaier.SimpleDlna.Utilities;
using System.IO;

namespace SimpleDlna.Tests
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class ServerDescriptionTest
  {

    [Test]
    public void ServerDescription_AdoptInfo_Test()
    {
      var data = new ServerDescription {
        Directories = new [] { "dir1", "dir2" },
        Name = "data",
        Order = "Order",
        FileStore = "FileStore",
        OrderDescending = false,
        Types = DlnaMediaTypes.All,
        Views = new [] { "Views1", "Views2" },
        Macs = new [] { "Mac1", "Mac2" },
        Ips = new [] { "IP1", "IP2" },
        UserAgents = new [] { "UserAgent1", "UserAgent2" }
      };
      var data2 = new ServerDescription();
      data2.AdoptInfo(data);
      Assert.AreEqual(data.Directories, data2.Directories);
      Assert.AreEqual(data.Name, data2.Name);
      Assert.AreEqual(data.Order, data2.Order);
      Assert.AreEqual(data.FileStore, data2.FileStore);
      Assert.AreEqual(data.OrderDescending, data2.OrderDescending);
      Assert.AreEqual(data.Types, data2.Types);
      Assert.AreEqual(data.Views, data2.Views);
      Assert.AreEqual(data.Macs, data2.Macs);
      Assert.AreEqual(data.Ips, data2.Ips);
      Assert.AreEqual(data.UserAgents, data2.UserAgents);
      var filenames = new[] { "ServerDescription.tmp1", "ServerDescription.tmp2" };
      try {
        XmlHelper.ToFile(data, filenames[0]);
        XmlHelper.ToFile(data2, filenames[1]);
        Assert.AreEqual(File.ReadAllText(filenames[0]), File.ReadAllText(filenames[1]));
      }
      finally {
        if (File.Exists(filenames[0])) File.Delete(filenames[0]);
        if (File.Exists(filenames[1])) File.Delete(filenames[1]);
      }
    }
  }
}
