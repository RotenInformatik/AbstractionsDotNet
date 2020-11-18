using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.Abstractions.Microsoft.Logging"),]
[assembly: AssemblyDescription("Infrastructure and cross-cutting-concern abstractions for .NET"),]
[assembly: Guid("9A1D556F-1E3A-48D8-8AD9-87FFA0A54504"),]

[assembly: AssemblyProduct("RotenInformatik/AbstractionsDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(false),]
[assembly: AssemblyVersion("1.2.0.0"),]
[assembly: AssemblyFileVersion("1.2.0.0"),]
[assembly: AssemblyInformationalVersion("1.2.0.0"),]

[assembly: InternalsVisibleTo("RI.Abstractions.Tests")]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
