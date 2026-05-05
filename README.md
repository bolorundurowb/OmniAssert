# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

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

Whether you're writing unit tests, integration tests, or behavior-driven scenarios, OmniAssert helps you express intent
clearly and diagnose failures quickly.

## Requirements

- **Runtime**: projects targeting **.NET 10** (or a compatible TFM) can reference the **`OmniAssert`** NuGet package. The runtime assembly name is **`OmniAssert.Core`**.
- **Optional interceptors**: **.NET 10 SDK**, **C# 14**, and the MSBuild properties in [Optional: richer boolean failures with interceptors](#optional-richer-boolean-failures-with-interceptors). Interceptors only affect `VerifyExpression(bool, string?)` (the `string` is normally supplied by `[CallerArgumentExpression]`).

## Install

Add the package to your test project:

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="1.0.0-alpha.1" />
</ItemGroup>
```

If you are building from this repository or a fork (project references instead of NuGet), see **[CONTRIBUTING.md](CONTRIBUTING.md)** for layout, build commands, and how to wire the analyzer like the sample project.

## Quick start

1. Reference **`OmniAssert`** (see [Install](#install)).
2. Add `using static OmniAssert.Assert;` so call sites stay short.
3. Write `Verify(…)` and `VerifyExpression(…)` as needed.

```csharp
using static OmniAssert.Assert;

[Fact]
public void Should_validate_user()
{
    Verify(user.Id).ToBeGreaterThan(0);
    Verify(user.Email).ToContain("@");
    Verify(user.IsActive).ToBeTrue();
}
```

## Public API

You use **`Assert`** (usually with `using static OmniAssert.Assert`), the fluent assertion structs it returns, **`AssertionScope`**, **`OmniAssertionException`**, and **`AssertionCapture`** (for advanced `VerifyExpression` scenarios). Types such as the diff engine and terminal colour helpers are **internal**—you exercise them through **`Verify(…).ToBeEquivalentTo(…)`** and other public assertions. When interceptors are enabled, generated helpers appear under **`OmniAssert.Generated`**.

## Core usage

```csharp
using static OmniAssert.Assert;

[Fact]
public void Example()
{
    Verify(answer).ToBe(42);
    Verify(name).ToContain("Acme");
    Verify(isReady).ToBeTrue();
}
```

## API at a glance

| Area | Pattern | Notes |
|------|---------|--------|
| **Values** | `Verify(x).ToBe(…)`, comparisons, ranges, `ToBeApproximately` | Numeric types use `INumber<T>`; `BigInteger` is included. |
| **Text** | `Verify(s).ToContain`, `ToMatch`, `ToStartWith`, `ToBe(…, StringComparison)` | Also `ToBeNull`, `NotToBeNull`, `ToBeNullOrWhiteSpace`, etc. |
| **Collections** | `Verify(list).ToContain`, `HasCount`, `AllSatisfy`, `ToBeEquivalentTo` | Equivalence is an unordered multiset comparison. |
| **Enums** | `Verify(e).ToBe(…)`, `NotToBe(…)` | |
| **Nullables** | `VerifyNullable(x).ToBeNull()` / `NotToBeNull()` | Separate overloads for class vs struct nullables. |
| **Objects** | `Verify(o).ToBeOfType<T>()`, `ToBeAssignableTo<T>()`, `ToBeEquivalentTo(…)` | Deep equivalence walks public properties and sequences and reports a structured diff. |
| **Dates** | `Verify(dt).ToBeAfter` / `ToBeBefore` / `ToBeWithin` | Works for `DateTime` and `DateTimeOffset`. |
| **Exceptions** | `Throws<T>(() => …)`, `NotThrow`, `ThrowsAsync`, `NotThrowAsync`, `CompleteWithin` | Chain `.WithMessage`, `.WithInnerException<TInner>()` on the returned assertion object. |
| **Soft asserts** | `using (new AssertionScope()) { … }` | Failures inside the scope are collected and thrown as one aggregate at scope end. |

## Common scenarios

### Assert a thrown exception

```csharp
Throws<InvalidOperationException>(() => service.Execute())
    .WithMessage("Operation is not valid*");
```

### Assert multiple outcomes in one test run

```csharp
using (new AssertionScope())
{
    Verify(user.Name).ToBe("John");
    Verify(user.Age).ToBeGreaterThan(17);
    Verify(user.Email).ToContain("@");
}
```

### Assert structural equivalence

```csharp
Verify(actualDto).ToBeEquivalentTo(expectedDto);
```

## Booleans: `Verify(bool)` vs `VerifyExpression(bool)`

- **`Verify(flag).ToBeTrue()`** or **`.ToBeFalse()`** — `Verify(bool)` returns `BoolAssertions`. This is the main fluent style.

```csharp
Verify(isValid).ToBeTrue();
Verify(featureDisabled).ToBeFalse();
```

- **`VerifyExpression(condition)`** — a single void call that asserts the condition is true, with structured failure text.

```csharp
VerifyExpression(x > 0 && count < limit);
```

Use `Verify(…).ToBeTrue()` when you want the same fluent style as other assertions. Use `VerifyExpression(…)` when a one-shot condition reads better, or when you enable interceptors (below).

## Optional: richer boolean failures with interceptors

Interceptors are **off** by default. When enabled, the bundled Roslyn generator emits stubs under **`OmniAssert.Generated`** so `VerifyExpression(bool, …)` call sites can be rewritten at compile time:

- a bare boolean identifier (including parenthesised) becomes `Verify(condition, expression).ToBeTrue()`;
- any other boolean shape becomes `VerifyExpression(condition, expression)` with the captured expression text (same failure path as the non-intercepted API).

Add the following to your test project **in addition to** the **`OmniAssert`** package reference from [Install](#install):

```xml
<PropertyGroup>
  <LangVersion>14</LangVersion>
  <OmniAssertEnableVerifyInterceptors>true</OmniAssertEnableVerifyInterceptors>
  <InterceptorsNamespaces>$(InterceptorsNamespaces);OmniAssert.Generated</InterceptorsNamespaces>
</PropertyGroup>

<ItemGroup>
  <CompilerVisibleProperty Include="OmniAssertEnableVerifyInterceptors" />
</ItemGroup>
```

- **`OmniAssertEnableVerifyInterceptors`**: `true` emits interceptors per syntax tree; `false` (or omitted) keeps `VerifyExpression` on the normal `[CallerArgumentExpression]` path.
- **`InterceptorsNamespaces`**: must include **`OmniAssert.Generated`** so the compiler applies emitted interceptors.

Only **`VerifyExpression(bool, string?)`** call sites are intercepted (not `Verify(bool).ToBeTrue()` fluent calls). A separate overload, **`VerifyExpression(bool, AssertionCapture)`**, is used by boolean lowering in tooling; it is **not** intercepted.

| Condition at the call site | What the interceptor does |
|----------------------------|-----------------------------|
| Bare identifier (including parenthesised) | `Verify(condition, expression).ToBeTrue()` |
| Operators, literals, `!flag`, compound logical expressions, … | `VerifyExpression(condition, expression)` |

## Optional: operand capture via source rewrite (advanced)

This path is intended for **advanced setups** that use the generator from source with **`OmniAssertEnableRewrite`** set to **`true`**, list a `.cs` file as an **`AdditionalFiles`** item, and exclude that file from **`Compile`**. The generator then emits a rewritten compilation unit that lowers `VerifyExpression` to `VerifyExpression(bool, AssertionCapture)` with per-subexpression values in the failure message. It is **not** required for normal NuGet consumption. Wiring details and a sample live in the repository; see **[CONTRIBUTING.md](CONTRIBUTING.md)** and **`src/samples/VerifyInterceptorsSample`**.

## More examples

```csharp
Verify(42).ToBe(42);
Verify(10).ToBeGreaterThan(5);
Verify(3.14159).ToBeApproximately(3.14, 0.01);

Verify("Hello World").ToContain("Hello");
Verify(email).ToMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");

Verify(users).ToContain(currentUser);
Verify(results).HasCount(3);

VerifyNullable(someObject).NotToBeNull();
Verify(MyEnum.On).ToBe(MyEnum.On);

Verify(instance).ToBeOfType<MyClass>();
Verify(actualDto).ToBeEquivalentTo(expectedDto);

Verify(dateTime).ToBeAfter(DateTime.UtcNow.AddDays(-1));

using (new AssertionScope())
{
    Verify(user.Name).ToBe("John");
    Verify(user.Age).ToBe(30);
}
```

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
