using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.Abstractions.Microsoft.DependencyInjection"),]
[assembly: AssemblyDescription("Infrastructure and cross-cutting-concern abstractions for .NET"),]
[assembly: Guid("CF628B48-B6E5-4339-B53D-ABCAB45A724F"),]

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
