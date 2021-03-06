# History

[![Nuget](https://img.shields.io/nuget/v/RI.Abstractions.Common)](https://www.nuget.org/packages/RI.Abstractions.Common/) [![License](https://img.shields.io/github/license/RotenInformatik/AbstractionsDotNet)](LICENSE) [![Repository](https://img.shields.io/badge/repo-AbstractionsDotNet-lightgrey)](https://github.com/RotenInformatik/AbstractionsDotNet) [![Documentation](https://img.shields.io/badge/docs-Readme-yellowgreen)](README.md) [![Documentation](https://img.shields.io/badge/docs-History-yellowgreen)](HISTORY.md) [![Documentation](https://img.shields.io/badge/docs-API-yellowgreen)](https://roteninformatik.github.io/AbstractionsDotNet/api/) [![Support me](https://img.shields.io/badge/support%20me-Ko--fi-ff69b4?logo=Ko-fi)](https://ko-fi.com/franziskaroten)

---

## 1.4.0

* [#6: Move thread dispatcher abstraction into this repository](https://github.com/RotenInformatik/AbstractionsDotNet/issues/6)
* [#23: ThrowIf... methods do not check for contract type](https://github.com/RotenInformatik/AbstractionsDotNet/issues/23)
* [#24: Insufficient tests of the builder (e.g. BuildStandalone)](https://github.com/RotenInformatik/AbstractionsDotNet/issues/24)
* [#25: AddDefault... methods call itself instead of Add... methods](https://github.com/RotenInformatik/AbstractionsDotNet/issues/25)
* [#26: BuilderBase.Build checks for ILogger instead for ICompositionContainer](https://github.com/RotenInformatik/AbstractionsDotNet/issues/26)
* [#27: Implement SimpleDispatcher](https://github.com/RotenInformatik/AbstractionsDotNet/issues/27)

## 1.3.0

* [#5: Create unit tests](https://github.com/RotenInformatik/AbstractionsDotNet/issues/5)
* [#18: Make ILogger implementations more robust](https://github.com/RotenInformatik/AbstractionsDotNet/issues/18)
* [#19: Prevent multiple calls to ICompositionContainer.Register by contract](https://github.com/RotenInformatik/AbstractionsDotNet/issues/19)
* [#20: SimpleContainer cannot create instances from type constructors](https://github.com/RotenInformatik/AbstractionsDotNet/issues/20)
* [#21: SimpleContainer does not adhere to AlwaysRegister flag](https://github.com/RotenInformatik/AbstractionsDotNet/issues/21)

## 1.2.0

* [#13: Add recursion detection when injecting constructor parameters](https://github.com/RotenInformatik/AbstractionsDotNet/issues/13)
* [#14: Add NullLogger and make ILogger registration optional](https://github.com/RotenInformatik/AbstractionsDotNet/issues/14)
* [#15: SimpleContainer does not properly adhere Transient registrations](https://github.com/RotenInformatik/AbstractionsDotNet/issues/15)

## 1.1.0

* [#7: Add list of implementations to readme](https://github.com/RotenInformatik/AbstractionsDotNet/issues/7)
* [#8: Add additional extension methods to IBuilderExtensions](https://github.com/RotenInformatik/AbstractionsDotNet/issues/8)
* [#9: Parameter alwaysRegister is ignored in IBuilderExtensions.AddTransient](https://github.com/RotenInformatik/AbstractionsDotNet/issues/9)
* [#10: Some IBuilderExtensions methods use BuilderBase instead of IBuilder](https://github.com/RotenInformatik/AbstractionsDotNet/issues/10)
* [#11: Add possibility to use builder without a composition container (independent/standalone build)](https://github.com/RotenInformatik/AbstractionsDotNet/issues/11)

## 1.0.0

First version.

Partial successor of [Decoupling & Utilities Framework](https://github.com/RotenInformatik/RI_Framework), which is being archived.
