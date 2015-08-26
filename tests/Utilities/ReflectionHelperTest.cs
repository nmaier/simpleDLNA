using NUnit.Framework;
using NMaier.SimpleDlna.FileMediaServer;
using System.IO;
using SimpleDlna.Tests.Mocks;
using NMaier.SimpleDlna.Utilities;
using NMaier.SimpleDlna.FileStore.SQLite;

namespace SimpleDlna.Tests.Utilities
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class ReflectionHelperTest
  {
    [Test]
    public void ReflectionHelper_Create_Test()
    {
      Assert.IsNotNull(ReflectionHelper.Create("assembly=SimpleDlna.FileStore.SQLite;type=NMaier.SimpleDlna.FileStore.SQLite.FileStore;"));
    }

    [Test]
    public void ReflectionHelper_StringToDictionary_Test()
    {
      var d = ReflectionHelper.StringToDictionary(string.Format("assembly={0};type={1};", typeof(FileStore).AssemblyQualifiedName, typeof(FileStore).FullName));
      Assert.IsTrue(d.ContainsKey("assembly"));
      Assert.IsTrue(d.ContainsValue(typeof(FileStore).AssemblyQualifiedName));
      Assert.IsTrue(d.ContainsKey("type"));
      Assert.IsTrue(d.ContainsValue(typeof(FileStore).FullName));
      Assert.AreEqual(typeof(FileStore).FullName, d["type"]);
      Assert.AreEqual(typeof(FileStore).AssemblyQualifiedName, d["assembly"]);
    }
  }
}
