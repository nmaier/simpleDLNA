using NUnit.Framework;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using System.Linq;
using tests.Mocks;
using NMaier.SimpleDlna.Server.Metadata;
using System;

namespace SimpleDlna.Tests
{
  /// <summary>
  /// ByDateViewTest transformation tests
  /// </summary>
  [TestFixture]
  public class ByDateViewTest
  {

    public class MediaResourceMockWithInfo : MediaResource, IMetaInfo
    {
      public DateTime InfoDate { get; set; }

      public long? InfoSize { get; set; }
    }

    [Test]
    public void ByDateView_RegisterFolder_Test()
    {
      var ids = new Identifiers(ComparerRepository.Lookup("title"), false);
      ids.AddView("bydate");
      var f = new MediaFolder();
      var addRes = new[] {
        new MediaResource() { Path = @"C:\somepath\resrouceZ.mkv", Title = "Z" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceY.mkv", Title = "Y" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceV.mkv", Title = "V" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceO.mkv", Title = "O" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceP.mkv", Title = "P" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceQ.mkv", Title = "Q" }
        ,new MediaResourceMockWithInfo() { Path = @"C:\somepath\resrouceM.mkv", Title = "M", InfoDate = new DateTime(2015,1,1) }
        ,new MediaResource() { Path = @"C:\somepath\resrouceE.mkv", Title = "E" }
      };
      f.AccessorChildItems.AddRange(addRes);
      f.AccessorChildFolders.Add(new MediaFolder() { Path = @"C:\somepath" });
      ids.RegisterFolder("tempid", f);
      var res = ids.Resources;
      Assert.IsTrue(res.Select(r => r.Target).ToList().Contains(addRes[2]), "Not contains added resource");
      Assert.IsNotNull(ids.GetItemByPath("/:/:2015-Jan"), @"GetItemByPath(""/:/:2015-Jan"") failed");
      Assert.AreEqual(ids.GetItemById(addRes[2].Id), addRes[2], "GetItemById(addRes[2].Id) failed");
     }
  }
}
