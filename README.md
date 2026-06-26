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
  <PackageReference Include="OmniAssert" Version="2.3.0" />
  <!-- Optional: domain validators (web, financial, security, regional) -->
  <PackageReference Include="OmniAssert.Extensions" Version="2.3.0" />
</ItemGroup>
```

```csharp
using OmniAssert;
using OmniAssert.Extensions.Web; // when using Extensions

user.Id.Must().BeGreaterThan(0);
user.Email.Must().BeEmailAddress();
(() => Service.Create(badInput)).Throws<ArgumentException>();
```

## Recently added assertions

Convenience assertions added on top of the core primitives:

**Strings** — `BeTrimmed()`, `HaveLengthBetween(min, max)`, `BeAlphanumeric()`, `BeLowerCase()`, `BeUpperCase()`, `BeNumeric()`, `BeBase64()`, `BeHexString()`, `BeHexColor()`, `BeIso8601()`, `BeDateString(format)`, `BeAbsolutePath()`, `BeRelativePath()`. String date/format validators parse with `CultureInfo.InvariantCulture` so they are culture-independent.

**Numbers** — `BePositive()`, `BeNegative()`, `BeEven()`, `BeOdd()`, `BeMultipleOf(n)`, `BeFinite()`, `BeInfinite()`, `HaveDecimalPlaces(n)`.

**Dates** — `DateTime`/`DateTimeOffset`: `BeInPast()`, `BeInFuture()`, `BeSameDayAs(expected)`. `DateOnly`: `BeToday()`, `BeYesterday()`, `BeTomorrow()`, `BeWeekday()`, `BeWeekend()`, `BeLeapYear()`, `BeWithinDays(n, anchor)`. `BeInPast`/`BeInFuture` compare against `UtcNow`, so subjects should be UTC; the `DateOnly` "today" helpers use the local clock and can shift across midnight.

**Collections** — `HaveCountBetween(min, max)` (also on spans), `ContainOnly(allowed)`, `ContainOnly(predicate)`, `BeSubsetOf(expected)`, `BeSupersetOf(expected)`. Set operations use default equality with set semantics.

**Files** — `NotBeEmpty()`, `HaveExtension(ext)` (leading dot optional, case-insensitive).

**Equivalence options** — `BeEquivalentTo(expected, EquivalenceOptions)` on collections and objects, with `IgnoreCase` and `IgnoreCollectionOrder` flags.

```csharp
order.Total.Must().BePositive();
sku.Must().BeAlphanumeric();
createdAt.Must().BeInPast();
roles.Must().BeSubsetOf(allRoles);
actual.Must().BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCase = true, IgnoreCollectionOrder = true });
```

### Suggested name → OmniAssert equivalent

Common assertion names from other libraries and their OmniAssert spelling:

| Suggested | OmniAssert |
|-----------|------------|
| `BeDistinct` / `HaveUniqueItems` | `BeUnique()` |
| `BeSorted` / `BeInAscendingOrder` | `BeInAscendingOrder()` |
| `BeSortedDescending` | `BeInDescendingOrder()` |
| `MatchRegex` / `BeRegex` | `Match(pattern)` |
| `BeNumericString` | `BeNumeric()` |
| `BeExistingFile` | `path.FileExists()` |
| `HaveSameElementsAs` | `BeEquivalentTo()` (unordered) / `BeSequenceEqual()` (ordered) |
| `BeQuasiEquivalentTo` | `BeEquivalentTo(expected, new EquivalenceOptions { IgnoreCase = true, IgnoreCollectionOrder = true })` |

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md).

## License

GNU General Public License v3.0 — see [LICENSE](LICENSE).
