namespace NMaier.SimpleDlna.Server.Views
{
  internal class DoubleKeyedVirtualFolder
    : KeyedVirtualFolder<SimpleKeyedVirtualFolder>
  {
    public DoubleKeyedVirtualFolder()
    {
    }

    public DoubleKeyedVirtualFolder(IMediaFolder aParent, string aName)
      : base(aParent, aName)
    {
    }
  }
}
