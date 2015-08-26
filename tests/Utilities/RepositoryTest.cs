using System;
using NUnit.Framework;
using NMaier.SimpleDlna.Server.Views;
using NMaier.SimpleDlna.Utilities;
using System.Linq;
using tests.Mocks;
using NMaier.SimpleDlna.FileMediaServer;
using NMaier.SimpleDlna.FileStore.SQLite;

namespace SimpleDlna.Tests.Utilities
{
  /// <summary>
  /// Summary description for FileServerTest
  /// </summary>
  [TestFixture]
  public class RepositoryTest
  {

    public sealed class ViewRepositoryMock : Repository<IView>
    {
    }

    [Test]
    public void Repository_GetAllTypes_Test()
    {
      var itype = typeof(IView);
      var types = AppDomain.CurrentDomain.GetAssemblies()
                         .SelectMany(a => a.GetTypes())
                         .Where(t => itype.IsAssignableFrom(t) && t.IsClass && !t.IsAbstract)
                         .ToList();
      Assert.IsTrue(types.Contains(typeof(View)));
    }

    [Test]
    public void Repository_ViewRepository_Lookup_Test()
    {
      var target = ViewRepositoryMock.Lookup(typeof(View).Name);
      Assert.IsNotNull(target);
    }

    [Test]
    public void Repository_ViewRepository_ListItems_Test()
    {
      var target = ViewRepositoryMock.ListItems();
      Assert.IsTrue(target.Count > 0);
    }

    [Test]
    public void Repository_FileStoreRepository_Lookup_Test()
    {
      var target = FileStoreRepository.Lookup((new NMaier.SimpleDlna.FileStore.SQLite.FileStore()).Name);
      Assert.IsNotNull(target);
    }

    [Test]
    public void Repository_FileStoreRepository_ListItems_Test()
    {
      var target = FileStoreRepository.ListItems();
      Assert.IsTrue(target.Count > 0);
      var keys = target.Keys.ToArray();
      var specificObject = new FileStore();
      Assert.IsTrue(keys.Contains(specificObject.Name));
    }


  }
}
