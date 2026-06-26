<div align="center">
  <img
    src="https://raw.githubusercontent.com/bolorundurowb/OmniAssert/refs/heads/master/assets/omni-assert-logo.svg"
    alt="omni assert logo"  />
  <h1 align="center">Omni Assert</h1>
</div>

<p align="center">
  <a href="https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml">
    <img src="https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg" alt="Build, Test & Coverage" />
  </a>
  <a href="https://codecov.io/gh/bolorundurowb/OmniAssert">
    <img src="https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA" alt="codecov" />
  </a>
  <a href="./LICENSE">
    <img src="https://img.shields.io/badge/license-GPLv3-orange.svg" alt="License" />
  </a>
</p>

**OmniAssert** is a fluent assertion library for .NET 10 and C# 14. Tests read like plain English and failures show the expression under test, actual values, and optional compile-time operand capture.

> Built with substantial AI assistance. Review the implementation before use in security-sensitive or compliance-heavy environments.

## Documentation

**[Full documentation](https://bolorundurowb.github.io/OmniAssert/)** — API reference, soft assertions, object diffing, interceptors, analyzer rules (OA001–OA007), v1 → v2 migration, and [Extensions](https://bolorundurowb.github.io/OmniAssert/#extensions).

## Key features

- **Fluent API** — `user.Email.Must().Contain("@")` reads naturally across strings, numbers, collections, objects, exceptions, files, dates, spans, and nullable values
- **Deep object comparison** — `BeEquivalentTo` compares object graphs recursively with hierarchical diffs
- **Soft assertions** — `AssertionScope` collects multiple failures and reports them together
- **Compile-time diagnostics** — optional Roslyn interceptors capture operand values when boolean expressions fail

## Quick start

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="2.1.0" />
  <!-- Optional: domain validators (web, financial, security, regional) -->
  <PackageReference Include="OmniAssert.Extensions" Version="2.1.0" />
</ItemGroup>
```

```csharp
using OmniAssert;
using OmniAssert.Extensions.Web; // when using Extensions

user.Id.Must().BeGreaterThan(0);
user.Email.Must().BeEmailAddress();
(() => Service.Create(badInput)).Throws<ArgumentException>();
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

GNU General Public License v3.0 — see [LICENSE](LICENSE).
