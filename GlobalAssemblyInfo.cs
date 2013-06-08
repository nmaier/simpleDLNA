using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

#if DEBUG
[assembly: AssemblyConfiguration("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany("Nils Maier")]
[assembly: AssemblyProduct("SimpleDLNA")]
[assembly: AssemblyCopyright("Copyright © 2012-2013 Nils Maier")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

[assembly: ComVisible(false)]

[assembly: AssemblyVersion("0.9.0.*")]
[assembly: NeutralResourcesLanguageAttribute("en-US")]
