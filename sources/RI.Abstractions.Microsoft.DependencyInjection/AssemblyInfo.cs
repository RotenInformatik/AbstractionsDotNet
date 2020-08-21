using System;
using System.Reflection;
using System.Runtime.InteropServices;




[assembly: AssemblyTitle("RI.Abstractions.Microsoft.DependencyInjection"),]
[assembly: AssemblyDescription("Infrastructure and cross-cutting-concern abstractions for .NET"),]
[assembly: Guid("20EBC452-7615-4D59-8581-6E98A7D2CDB6"),]

[assembly: AssemblyProduct("RotenInformatik/AbstractionsDotNet"),]
[assembly: AssemblyCompany("Roten Informatik"),]
[assembly: AssemblyCopyright("Copyright (c) 2017-2020 Roten Informatik"),]
[assembly: AssemblyTrademark(""),]
[assembly: AssemblyCulture(""),]
[assembly: CLSCompliant(false),]
[assembly: AssemblyVersion("1.0.0.0"),]
[assembly: AssemblyFileVersion("1.0.0.0"),]
[assembly: AssemblyInformationalVersion("1.0.0.0"),]

#if DEBUG
[assembly: AssemblyConfiguration("DEBUG"),]
#else
[assembly: AssemblyConfiguration("RELEASE")]
#if !RELEASE
#warning "RELEASE not specified"
#endif
#endif
