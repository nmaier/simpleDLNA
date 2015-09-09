using NMaier.SimpleDlna.Server;
using NMaier.SimpleDlna.Server.Views;
using NMaier.SimpleDlna.Utilities;
using System;

namespace tests.Mocks
{
  public class View : IView
    {
      public string Description
      {
        get
        {
          return string.Format("[{0}] - Description", this.GetType().Name);
        }
      }

      public string Name
      {
        get
        {
          return this.GetType().Name;
        }
      }

      public void SetParameters(AttributeCollection parameters)
      {
        throw new NotImplementedException();
      }

      public IMediaFolder Transform(IMediaFolder root)
      {
        return root;
      }
    }
}
