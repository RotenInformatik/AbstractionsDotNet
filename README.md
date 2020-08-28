# RI.Abstractions

[![Nuget](https://img.shields.io/nuget/v/RI.Abstractions.Common)](https://www.nuget.org/packages/RI.Abstractions.Common/) [![License](https://img.shields.io/github/license/RotenInformatik/AbstractionsDotNet)](LICENSE) [![Repository](https://img.shields.io/badge/repo-AbstractionsDotNet-lightgrey)](https://github.com/RotenInformatik/AbstractionsDotNet) [![Documentation](https://img.shields.io/badge/docs-Readme-yellowgreen)](README.md) [![Documentation](https://img.shields.io/badge/docs-History-yellowgreen)](HISTORY.md) [![Documentation](https://img.shields.io/badge/docs-API-yellowgreen)](https://roteninformatik.github.io/AbstractionsDotNet/api/) [![Support me](https://img.shields.io/badge/support%20me-Ko--fi-ff69b4?logo=Ko-fi)](https://ko-fi.com/andreasroten)

---

Infrastructure and cross-cutting-concern abstractions for .NET.

Based on .NET Standard 2.0, with minimal dependencies.

---

These abstractions are mainly used by other projects/libraries worked on by [RotenInformatik](https://github.com/RotenInformatik/), both public and private ones.

Therefore, these abstractions are tailored for the needs of those projects/libraries - but anyone can use them.

Currently, the following abstractions are implemented:

* **Builder** (boilerplate for implementing the builder pattern)
* **Composition container** (registration abstraction for dependency injection)
* **Logging** (simple logging abstraction)

Currently, the following is being abstracted:

* **Microsoft.Extensions.Logging**
* **Microsoft.Extensions.DependencyInjection**
