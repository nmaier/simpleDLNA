using System;
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

[assembly: AssemblyVersion("0.12.*")]
[assembly: AssemblyInformationalVersion("0.12 - Hello Dexter Morgan")]

[assembly: NeutralResourcesLanguageAttribute("en-US")]

[assembly: CLSCompliant(true)]
