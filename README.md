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

## Introduction

**OmniAssert** is a modern, fluent assertion library for .NET that makes your test code more readable and your test
failures more informative. Built for **.NET 10** and **C# 14**, it combines the best ideas from established assertion
frameworks while adding powerful compile-time features that help you write better tests faster.

> **Provenance:** OmniAssert was built with **substantial help from AI-assisted coding tools**. If you are evaluating it for production, security-sensitive, or compliance-heavy use, apply the same scrutiny you would to any rapidly authored codebase: read critical paths, run your own tests, and satisfy your organisation’s review and supply-chain policies.

> **Upgrading from v1?** See **[MIGRATION.md](MIGRATION.md)** for the `Ensure` / `Must()` / `Be*` API and automated Roslyn migration.

At its core, OmniAssert provides an expressive, natural-language API for assertions: `user.Email.Must().Contain("@")`
reads like plain English. When assertions fail, you get rich, detailed error messages that include the actual and
expected values, the expression that failed, and contextual information about what went wrong—making debugging faster
and less frustrating.

### What sets OmniAssert apart

- **Deep object comparison**: The `BeEquivalentTo` assertion performs intelligent structural comparison of complex
  object graphs, recursively comparing public properties, handling collections as unordered multisets, and safely
  navigating circular references. When objects don't match, you get a clear, hierarchical diff showing exactly where
  they diverge.

- **Soft assertions with AssertionScope**: Wrap multiple assertions in an `AssertionScope` to collect all failures in a
  test before reporting them together. No more fixing one assertion at a time—see everything that's wrong in one test
  run.

- **Compile-time assertion enhancement** *(optional)*: Enable Roslyn interceptors to get richer diagnostics for boolean
  expressions. When `VerifyExpression(x > 0 && count < limit)` fails, you see not just "condition was false" but the
  actual values of `x`, `count`, and `limit`. An advanced rewrite mode can even capture intermediate sub-expression
  values for complex logical conditions.

- **Automated migration from v1**: Bundled Roslyn analyzers (OA001–OA003) detect legacy `Assert` / `Verify()` / `To*`
  syntax and offer code fixes, including **Fix all occurrences in Solution**. See [MIGRATION.md](MIGRATION.md).

- **Comprehensive API coverage**: From basic equality and comparison checks to collection assertions, exception
  testing (sync and async), type checks, string pattern matching, date/time comparisons, nullable handling, and more—all
  with consistent, fluent syntax.

- **Exceptional diagnostics**: Leveraging `[CallerArgumentExpression]` and optional source generators, OmniAssert
  captures the text of your assertion expressions automatically, so failure messages show you exactly what you tested,
  not just that something failed.

- **Rich value formatting**: When assertions fail, OmniAssert provides clear, highlighted output using ANSI colours.
  Complex types like collections are automatically formatted to show their contents (up to 10 items) rather than just the
  type name, making it easier to see exactly what caused the mismatch.

Whether you're writing unit tests, integration tests, or behaviour-driven scenarios, OmniAssert helps you express intent
clearly and diagnose failures quickly.

## Requirements

- **Runtime**: projects targeting **.NET 10** (or a compatible TFM) can reference the **`OmniAssert`** NuGet package. The runtime assembly name is **`OmniAssert.Core`**.
- **Optional interceptors**: **.NET 10 SDK**, **C# 14**, and the MSBuild properties in [Compile-time boolean diagnostics (interceptors)](#compile-time-boolean-diagnostics-interceptors). Interceptors only affect `VerifyExpression(bool, string?)` (the `string` is normally supplied by `[CallerArgumentExpression]`).

## Install

Add the package to your test project:

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="2.0.0" />
</ItemGroup>
```

If you are building from this repository or a fork (project references instead of NuGet), see **[CONTRIBUTING.md](CONTRIBUTING.md)** for layout, build commands, and how to wire the analyzer like the sample project.

## Quick start

1. Reference **`OmniAssert`** (see [Install](#install)).
2. Write `.Must()` on any value or expression.
3. For booleans, you can also use `.VerifyExpression()`.

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

## Assertion Types

OmniAssert provides a rich set of extension methods for various types. Fluent assertions begin with `.Must()`; nullable
subjects use `.VerifyNullable()`.

### Basic Values & Numeric
Assertions for all standard numeric types (including `BigInteger`).

```csharp
answer.Must().Be(42);
count.Must().BeGreaterThan(0);
price.Must().BeApproximately(9.99, 0.01);
score.Must().BeOneOf(10, 20, 30);
```

- `Be(expected)` / `NotBe(unexpected)`
- `BeGreaterThan(threshold)` / `BeGreaterThanOrEqualTo(threshold)`
- `BeLessThan(threshold)` / `BeLessThanOrEqualTo(threshold)`
- `BeInRange(min, max)`
- `BeApproximately(expected, precision)`
- `BeOneOf(values...)`

### Strings
Fluent assertions for text analysis.

```csharp
name.Must().StartWith("John");
email.Must().Match(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
bio.Must().HaveLengthGreaterThan(10);
role.Must().BeIgnoringCase("admin");
```

- `Contain(substring)` / `NotContain(substring)`
- `StartWith(prefix)` / `EndWith(suffix)` / `NotEndWith(suffix)`
- `Match(regex)`
- `BeIgnoringCase(expected)` / `BeOneOf(values...)`
- `BeNull()` / `NotBeNull()`
- `BeEmpty()` / `NotBeEmpty()`
- `BeNullOrEmpty()` / `NotBeNullOrEmpty()`
- `BeNullOrWhiteSpace()` / `NotBeNullOrWhiteSpace()`
- `BeWhiteSpace()` / `NotBeWhiteSpace()`
- `HaveLength(expected)` / `HaveLengthGreaterThan(n)` / `HaveLengthLessThan(n)`

### Collections & Dictionaries
Deep verification for sequences and key-value pairs.

```csharp
users.Must().HaveCount(3);
items.Must().Contain(targetItem);
results.Must().BeUnique();
settings.Must().ContainKey("Theme");
```

- `HaveCount(n)`
- `HaveCountGreaterThan(n)` / `HaveCountLessThan(n)`
- `HaveCountMatching(n, predicate)`
- `BeEmpty()` / `NotBeEmpty()`
- `Contain(item)` / `NotContain(item)`
- `Contain(predicate)` / `AllSatisfy(predicate)` / `AnySatisfy(predicate)` / `NoneSatisfy(predicate)`
- `BeUnique()` / `HaveUniqueCount(n)`
- `BeInAscendingOrder()` / `BeInDescendingOrder()`
- `BeEquivalentTo(other)` (Multiset equivalence—same elements, order ignored)
- `BeSequenceEqual(other)` (Exact sequence equivalence—same elements in same order)
- `ContainInOrder(items...)` (Verifies items appear in the given order, not necessarily consecutively)
- `BeNull()` / `NotBeNull()`
- `ContainKey(key)` / `NotContainKey(key)` / `ContainValue(value)` / `NotContainValue(value)` / `HaveValue(key, value)`

### Objects & Equivalence
Verify object identity, types, and structural equality.

```csharp
instance.Must().BeOfType<MyClass>();
actualDto.Must().BeEquivalentTo(expectedDto);
```

- `Be(expected)` (Reference equality)
- `BeNull()` / `NotBeNull()`
- `BeOfType<T>()` / `NotBeOfType<T>()`
- `BeAssignableTo<T>()` / `NotBeAssignableTo<T>()`
- `BeEquivalentTo(expected)` (Recursive property comparison with diff)

### Exceptions & Tasks
Verify synchronous and asynchronous code behavior.

```csharp
// Synchronous
Action act = () => service.DoWork();
act.Throws<InvalidOperationException>().WithMessage("Failed");

// Asynchronous
Func<Task> asyncAct = () => service.DoWorkAsync();
await asyncAct.ThrowsAsync<ArgumentException>();

// Timeouts
await TimeSpan.FromSeconds(2).CompleteWithin(() => longRunningTask);
```

- `Throws<T>()` / `ThrowsAsync<T>()`
- `NotThrow()` / `NotThrowAsync()`
- `WithMessage(expected)` / `WithMessageContaining(substring)` / `WithMessageMatching(wildcard)`
- `WithMessageIgnoringCase(expected)`
- `WithInnerException<TInner>()`
- `CompleteWithin(action)`

### Dates, Times & Other Types
Assertions for temporal types, GUIDs, Enums, and URIs.

```csharp
dateTime.Must().BeAfter(yesterday);
timeSpan.Must().BePositive();
guid.Must().NotBeEmpty();
uri.Must().HaveHost("github.com");
```

- `DateTime` / `DateTimeOffset`: `BeBefore`, `BeAfter`, `BeWithin`
- `DateOnly` / `TimeOnly`: `BeBefore`, `BeAfter`
- `TimeSpan`: `BePositive`, `BeNegative`
- `Guid`: `BeEmpty`, `NotBeEmpty`
- `Enum`: `Be`, `BeOneOf`
- `Uri`: `HaveScheme`, `HaveHost`, `HavePath`, `HaveQuery`

### File System
Verify files and directories exist and check their properties.

```csharp
"config.json".FileExists().HaveContent(expectedJson);
"temp".DirectoryExists().BeEmpty();
```

- `FileExists()` -> `HaveContent(text)`, `BeEmpty()`
- `DirectoryExists()` -> `BeEmpty()`

## Soft Assertions: `AssertionScope`

Use `AssertionScope` to collect multiple failures and report them all at once when the scope is disposed.

```csharp
using (new AssertionScope())
{
    user.Name.Must().Be("John");
    user.Age.Must().Be(30);
    user.Email.Must().Contain("@");
}
```

## Booleans: `Must()` vs `VerifyExpression()`

OmniAssert provides two ways to assert boolean conditions:

1. **Fluent style**: `flag.Must().BeTrue()` — consistent with other assertions.
2. **Expression style**: `condition.VerifyExpression()` — best for complex logical conditions.

```csharp
// Fluent
isValid.Must().BeTrue();

// Expression (with rich diagnostics via bundled interceptors)
(x > 0 && count < limit).VerifyExpression();
```

## Compile-time boolean diagnostics (interceptors)

The bundled Roslyn generator is **on by default**. When active, it rewrites `VerifyExpression()` call sites at compile time to capture sub-expression values, so failure messages show the individual operands—not just "condition was false".

To opt out, add the following to your test project:

```xml
<PropertyGroup>
  <OmniAssertDisableVerifyInterceptors>true</OmniAssertDisableVerifyInterceptors>
</PropertyGroup>
```

> **C# 14 / .NET 10 requirement**: interceptors require `LangVersion` ≥ 14 and the `InterceptorsNamespaces` property. Both are met automatically when targeting .NET 10 with the default SDK settings, but if you pin an earlier language version you must add:
>
> ```xml
> <PropertyGroup>
>   <LangVersion>14</LangVersion>
>   <InterceptorsNamespaces>$(InterceptorsNamespaces);OmniAssert.Generated</InterceptorsNamespaces>
> </PropertyGroup>
> ```

When active:
- `flag.VerifyExpression()` (bare identifier) becomes `flag.Must(expression).BeTrue()`.
- `(complexExpr).VerifyExpression()` captures all operands so failure messages show each value.

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
