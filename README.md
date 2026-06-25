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

**OmniAssert** is a modern, fluent assertion library for .NET 10 and C# 14. Write tests that read like plain English
and get rich failure messages with actual values, captured expressions, and optional compile-time boolean diagnostics.

> OmniAssert was built with substantial help from AI-assisted coding tools. If you use it in production,
> security-sensitive, or compliance-heavy environments, review the implementation and apply your organisation’s
> supply-chain policies.

## Documentation

For installation, the full assertion API, soft assertions, object diffing, compile-time interceptors, and the
**v1 → v2 migration guide**, see the **[OmniAssert documentation](https://bolorundurowb.github.io/OmniAssert/)**.

## Key features

- **Fluent API** — `user.Email.Must().Contain("@")` reads naturally across strings, numbers, collections, objects, exceptions, files, dates, spans, and nullable values
- **Deep object comparison** — `BeEquivalentTo` compares object graphs recursively with hierarchical diffs
- **Soft assertions** — `AssertionScope` collects multiple failures and reports them together
- **Compile-time diagnostics** — optional Roslyn interceptors capture operand values when boolean expressions fail
- **Automated v1 migration** — bundled analyzers (OA001–OA004) with code fixes and fix-all-in-solution support

## Quick start

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="2.0.0" />
</ItemGroup>
```

```csharp
using OmniAssert;

[Fact]
public void Should_validate_user()
{
    user.Id.Must().BeGreaterThan(0);
    user.Email.Must().Contain("@");
    user.IsActive.Must().BeTrue();
}
```

## Basic example

A minimal xUnit test showing the usual flow: assert values with `.Must()`, then verify an action throws the expected exception.

```csharp
using OmniAssert;

public class OrderServiceTests
{
    [Fact]
    public void CreateOrder_with_invalid_email_fails()
    {
        var email = "not-an-email";

        email.Must().NotBeNullOrEmpty();
        email.Must().Contain("@");

        var ex = (() => OrderService.Create(email)).Throws<ArgumentException>();
        ex.WithMessageContaining("invalid email");
    }
}
```

When an assertion fails, the message includes the expression under test (for example `email`) and the actual value—not just “assertion failed.”

For requirements, every assertion type, exception testing, interceptors, and upgrading from v1, see the
[documentation](https://bolorundurowb.github.io/OmniAssert/).

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md) for development setup and pull request guidance.

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
