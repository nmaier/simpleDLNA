using NUnit.Framework;
using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Comparers;
using System.Linq;
using tests.Mocks;

namespace tests
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class ByTitleViewTest
  {

    [Test]
    public void ByTitleView_RegisterFolder_Test()
    {
      var ids = new Identifiers(ComparerRepository.Lookup("title"), false);
      ids.AddView("bytitle");
      var f = new MediaFolder();
      var addRes = new[] {
        new MediaResource() { Path = @"C:\somepath\resrouceZ.mkv", Title = "Z" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceY.mkv", Title = "Y" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceV.mkv", Title = "V" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceO.mkv", Title = "O" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceP.mkv", Title = "P" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceQ.mkv", Title = "Q" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceM.mkv", Title = "M" }
        ,new MediaResource() { Path = @"C:\somepath\resrouceE.mkv", Title = "E" }
      };
      f.AccessorChildItems.AddRange(addRes);
      f.AccessorChildFolders.Add(new MediaFolder() { Path = @"C:\somepath" });
      ids.RegisterFolder("tempid", f);
      var res = ids.Resources;
      Assert.IsTrue(res.Select(r => r.Target).ToList().Contains(addRes[2]), "Not contains added resource");
      Assert.IsNotNull(ids.GetItemByPath("/:/:Q"), "GetItemByPath(\"/:/:Q\") failed");
      Assert.AreEqual(ids.GetItemById(addRes[2].Id), addRes[2]);
      //target.Load();
    }
  }
}
