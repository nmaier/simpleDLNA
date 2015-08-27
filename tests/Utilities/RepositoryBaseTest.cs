using NUnit.Framework;
using NMaier.SimpleDlna.Utilities;

namespace SimpleDlna.Tests.Utilities
{
  /// <summary>
  /// ByTitleView transformation tests
  /// </summary>
  [TestFixture]
  public class RepositoryBaseTest
  {
    [Test]
    public void RepositoryBase_GetDomainAssemblies_Test()
    {
      var a = RepositoryBase.GetDomainAssemblies();
      Assert.IsTrue(a.Length > 0);
    }
  }
}
