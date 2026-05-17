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

> **Provenance:** OmniAssert was built with **substantial help from AI-assisted coding tools**. If you are evaluating it for production, security-sensitive, or compliance-heavy use, apply the same scrutiny you would to any rapidly authored codebase: read critical paths, run your own tests, and satisfy your organisation’s review and supply-chain policies.

## Introduction

**OmniAssert** is a modern, fluent assertion library for .NET that makes your test code more readable and your test
failures more informative. Built for **.NET 10** and **C# 14**, it combines the best ideas from established assertion
frameworks while adding powerful compile-time features that help you write better tests faster.

At its core, OmniAssert provides an expressive, natural-language API for assertions: `Verify(user.Email).ToContain("@")`
reads like plain English. When assertions fail, you get rich, detailed error messages that include the actual and
expected values, the expression that failed, and contextual information about what went wrong—making debugging faster
and less frustrating.

### What sets OmniAssert apart

- **Deep object comparison**: The `ToBeEquivalentTo` assertion performs intelligent structural comparison of complex
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
- **Optional interceptors**: **.NET 10 SDK**, **C# 14**, and the MSBuild properties in [Optional: richer boolean failures with interceptors](#optional-richer-boolean-failures-with-interceptors). Interceptors only affect `VerifyExpression(bool, string?)` (the `string` is normally supplied by `[CallerArgumentExpression]`).

## Install

Add the package to your test project:

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="1.0.1" />
</ItemGroup>
```

If you are building from this repository or a fork (project references instead of NuGet), see **[CONTRIBUTING.md](CONTRIBUTING.md)** for layout, build commands, and how to wire the analyzer like the sample project.

## Quick start

1. Reference **`OmniAssert`** (see [Install](#install)).
2. Write `.Verify(…)` on any value or expression.
3. For booleans, you can also use `.VerifyExpression()`.

```csharp
using OmniAssert;

[Fact]
public void Should_validate_user()
{
    user.Id.Verify().ToBeGreaterThan(0);
    user.Email.Verify().ToContain("@");
    user.IsActive.Verify().ToBeTrue();
}
```

## Assertion Types

OmniAssert provides a rich set of extension methods for various types. All assertions begin with `.Verify()` or specialized methods like `.VerifyNullable()`.

### Basic Values & Numeric
Assertions for all standard numeric types (including `BigInteger`).

```csharp
answer.Verify().ToBe(42);
count.Verify().ToBeGreaterThan(0);
price.Verify().ToBeApproximately(9.99, 0.01);
score.Verify().ToBeOneOf(10, 20, 30);
```

- `ToBe(expected)` / `NotToBe(unexpected)`
- `ToBeGreaterThan(threshold)` / `ToBeGreaterThanOrEqualTo(threshold)`
- `ToBeLessThan(threshold)` / `ToBeLessThanOrEqualTo(threshold)`
- `ToBeInRange(min, max)`
- `ToBeApproximately(expected, precision)`
- `ToBeOneOf(values...)`

### Strings
Fluent assertions for text analysis.

```csharp
name.Verify().ToStartWith("John");
email.Verify().ToMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
bio.Verify().HasLengthGreaterThan(10);
role.Verify().ToBeIgnoringCase("admin");
```

- `ToContain(substring)` / `NotToContain(substring)`
- `ToStartWith(prefix)` / `ToEndWith(suffix)`
- `ToMatch(regex)`
- `ToBeIgnoringCase(expected)` / `ToBeOneOf(values...)`
- `ToBeNull()` / `NotToBeNull()`
- `ToBeEmpty()` / `NotToBeEmpty()`
- `ToBeNullOrEmpty()` / `ToBeNullOrWhiteSpace()` / `NotToBeNullOrWhiteSpace()`
- `HasLength(expected)` / `HasLengthGreaterThan(n)` / `HasLengthLessThan(n)`

### Collections & Dictionaries
Deep verification for sequences and key-value pairs.

```csharp
users.Verify().HasCount(3);
items.Verify().ToContain(targetItem);
results.Verify().ToBeUnique();
settings.Verify().ContainKey("Theme");
```

- `HasCount(n)` / `ToHaveCount(n)`
- `HasCountGreaterThan(n)` / `HasCountLessThan(n)`
- `HasCountMatching(n, predicate)`
- `ToBeEmpty()` / `NotToBeEmpty()`
- `ToContain(item)` / `NotToContain(item)`
- `ToContain(predicate)` / `AllSatisfy(predicate)` / `AnySatisfy(predicate)` / `NoneSatisfy(predicate)`
- `ToBeUnique()` / `HasUniqueCount(n)`
- `ToBeInAscendingOrder()` / `ToBeInDescendingOrder()`
- `ToBeEquivalentTo(other)` (Multiset equivalence—same elements, order ignored)
- `ContainKey(key)` / `NotContainKey(key)` / `ContainValue(value)` / `NotContainValue(value)` / `HaveValue(key, value)`

### Objects & Equivalence
Verify object identity, types, and structural equality.

```csharp
instance.Verify().ToBeOfType<MyClass>();
actualDto.Verify().ToBeEquivalentTo(expectedDto);
```

- `ToBe(expected)` (Reference equality)
- `ToBeNull()` / `NotToBeNull()`
- `ToBeOfType<T>()` / `NotToBeOfType<T>()`
- `ToBeAssignableTo<T>()` / `NotToBeAssignableTo<T>()`
- `ToBeEquivalentTo(expected)` (Recursive property comparison with diff)

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
- `WithMessage(expected)` / `WithMessageContaining(substring)`
- `WithInnerException<TInner>()`
- `CompleteWithin(action)`

### Dates, Times & Other Types
Assertions for temporal types, GUIDs, Enums, and URIs.

```csharp
dateTime.Verify().ToBeAfter(yesterday);
timeSpan.Verify().ToBePositive();
guid.Verify().NotToBeEmpty();
uri.Verify().HaveHost("github.com");
```

- `DateTime` / `DateTimeOffset`: `ToBeBefore`, `ToBeAfter`, `ToBeWithin`
- `DateOnly` / `TimeOnly`: `ToBeBefore`, `ToBeAfter`
- `TimeSpan`: `ToBePositive`, `ToBeNegative`
- `Guid`: `ToBeEmpty`, `NotToBeEmpty`
- `Enum`: `ToBe`, `ToBeOneOf`
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
    user.Name.Verify().ToBe("John");
    user.Age.Verify().ToBe(30);
    user.Email.Verify().ToContain("@");
}
```

## Booleans: `Verify()` vs `VerifyExpression()`

OmniAssert provides two ways to assert boolean conditions:

1. **Fluent style**: `flag.Verify().ToBeTrue()` - consistent with other assertions.
2. **Expression style**: `condition.VerifyExpression()` - best for complex logical conditions.

```csharp
// Fluent
isValid.Verify().ToBeTrue();

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
- `flag.VerifyExpression()` (bare identifier) becomes `flag.Verify(expression).ToBeTrue()`.
- `(complexExpr).VerifyExpression()` captures all operands so failure messages show each value.

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
