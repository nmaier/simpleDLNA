namespace NMaier.SimpleDlna.Server
{
  internal interface IPrefixHandler : IHandler
  {
    string Prefix { get; }
  }
}
