# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

---

## For consumers

OmniAssert is a small, fluent assertion library for **.NET** tests.

Its design priorities are:

- concise assertions (`Verify(subject).To...`)
- useful failure messages (including caller expression text via `[CallerArgumentExpression]`)
- predictable behavior in both single-assert and scoped-assert flows

Failed checks throw `OmniAssertionException`. If you use `AssertionScope`, failures are collected and emitted together when the scope is disposed.

### Consumer quick start

1. Install the runtime package.
2. Import `OmniAssert.Assert` statically.
3. Write assertions with `Verify(...)`.

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

### What you need

- **Runtime**: projects targeting **.NET 10** (or compatible) can reference the `OmniAssert` NuGet package or `OmniAssert.Core` directly.
- **Optional boolean interceptors**: require **.NET 10 SDK**, **C# 14**, and MSBuild setup (shown below). Interceptors only affect `VerifyExpression(bool)`.

### Install the runtime

**From NuGet** (package id `OmniAssert`, assembly `OmniAssert.Core`):

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="1.0.0-alpha.0" />
</ItemGroup>
```

**From this repository**, reference `src\OmniAssert.Core\OmniAssert.Core.csproj`.

Pin the version you want from NuGet; the snippet above may lag the latest release.

### Core usage pattern

Import static members of `OmniAssert.Assert` so call sites stay short:

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

### Core API (at a glance)

| Area | Pattern | Notes |
|------|---------|--------|
| **Values** | `Verify(x).ToBe(…)`, comparisons, ranges, `ToBeApproximately` | Numeric types use `INumber<T>`; `BigInteger` is included. |
| **Text** | `Verify(s).ToContain`, `ToMatch`, `ToStartWith`, `ToBe(…, StringComparison)` | Also `ToBeNull`, `NotToBeNull`, `ToBeNullOrWhiteSpace`, etc. |
| **Collections** | `Verify(list).ToContain`, `HasCount`, `AllSatisfy`, `ToBeEquivalentTo` | Equivalence is unordered multiset comparison. |
| **Enums** | `Verify(e).ToBe(…)`, `NotToBe(…)` | |
| **Nullables** | `VerifyNullable(x).ToBeNull()` / `NotToBeNull()` | Separate overloads for class vs struct nullables. |
| **Objects** | `Verify(o).ToBeOfType<T>()`, `ToBeAssignableTo<T>()`, `ToBeEquivalentTo(…)` | Deep equivalence walks public properties and reports a diff. |
| **Dates** | `Verify(dt).ToBeAfter` / `ToBeBefore` / `ToBeWithin` | Works for `DateTime` and `DateTimeOffset`. |
| **Exceptions** | `Throws<T>(() => …)`, `NotThrow`, `ThrowsAsync`, `NotThrowAsync`, `CompleteWithin` | Chain `.WithMessage`, `.WithInnerException<TInner>()` on the returned assertion object. |
| **Soft asserts** | `using (new AssertionScope()) { … }` | Failures inside the scope are collected and thrown as one aggregate at scope end. |

### Common consumer scenarios

#### Assert a thrown exception

```csharp
Throws<InvalidOperationException>(() => service.Execute())
    .WithMessage("Operation is not valid*");
```

#### Assert multiple outcomes in one test run

```csharp
using (new AssertionScope())
{
    Verify(user.Name).ToBe("John");
    Verify(user.Age).ToBeGreaterThan(17);
    Verify(user.Email).ToContain("@");
}
```

#### Assert structural equivalence

```csharp
Verify(actualDto).ToBeEquivalentTo(expectedDto);
```

### Booleans: `Verify(bool)` vs `VerifyExpression(bool)`

- **`Verify(flag).ToBeTrue()`** or `.ToBeFalse()` — `Verify(bool)` returns `BoolAssertions`. This is the primary fluent style.

```csharp
Verify(isValid).ToBeTrue();
Verify(featureDisabled).ToBeFalse();
```

- **`VerifyExpression(condition)`** — one void call that asserts the condition is true immediately, with structured failure text.

```csharp
VerifyExpression(x > 0 && count < limit);
```

Use `Verify(...).ToBeTrue()` when you want consistency with other fluent assertions. Use `VerifyExpression(...)` when a single-call condition reads better, or when you enable interceptors.

### Optional: richer boolean failures with interceptors

Interceptors are off by default. When enabled, the `OmniAssert.Generator` analyzer emits stubs under `OmniAssert.Generated` so `VerifyExpression(bool, …)` call sites can be rewritten at compile time (simple identifiers → fluent `Verify(...).ToBeTrue()`; other expressions → internal `VerifyBoolean` with the expression string).

Add the analyzer as a **project reference** (not a normal assembly reference), then match `src\samples\VerifyInterceptorsSample\VerifyInterceptorsSample.csproj`:

```xml
<PropertyGroup>
  <LangVersion>14</LangVersion>
  <OmniAssertEnableVerifyInterceptors>true</OmniAssertEnableVerifyInterceptors>
  <InterceptorsNamespaces>$(InterceptorsNamespaces);OmniAssert.Generated</InterceptorsNamespaces>
</PropertyGroup>

<ItemGroup>
  <CompilerVisibleProperty Include="OmniAssertEnableVerifyInterceptors" />
</ItemGroup>

<ItemGroup>
  <ProjectReference Include="path\to\OmniAssert.Core\OmniAssert.Core.csproj" />
  <ProjectReference Include="path\to\OmniAssert.Generator\OmniAssert.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

- `OmniAssertEnableVerifyInterceptors`: `true` emits interceptors per syntax tree; `false` (or omitted) keeps `VerifyExpression` on the normal `[CallerArgumentExpression]` path.
- `InterceptorsNamespaces`: must include `OmniAssert.Generated` so the compiler applies emitted interceptors.

Only `VerifyExpression` call sites are intercepted, not `Verify(bool).ToBeTrue()` fluent calls.

| Condition at the call site | What the interceptor does |
|----------------------------|-----------------------------|
| Bare identifier (including parenthesized) | `Verify(condition, expression).ToBeTrue()` |
| Operators, literals, `!flag`, compound logical expressions, … | `VerifyBoolean(condition, new AssertionCapture(expression, null))` |

### More examples

Numeric, strings, collections, nullables, enums, objects, dates, and async helpers:

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

## For contributors

This section is for people building or changing OmniAssert itself. If you only consume the package, you can stop here.

### Contribution workflow

1. Fork and clone the repository.
2. Build and run tests locally.
3. Make focused changes with tests.
4. Open a PR with a clear description of behavior changes.

### Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime: `Assert`, assertion structs, `ObjectDiffWalker`, `AssertionScope`, `OmniAssertionException`, `AssertionCapture`. |
| `src/OmniAssert.Generator` | Roslyn incremental generator: `VerifyExpression(bool)` interceptors when `OmniAssertEnableVerifyInterceptors` is `true`. |
| `src/OmniAssert.Generator/Rewrite` | `VerifyExpansionEngine`: lowers boolean trees for tests and tooling. |
| `src/OmniAssert.Tests` | Main tests (interceptors enabled). |
| `src/OmniAssert.Tests.NoInterceptors` | Smoke tests (interceptors disabled). |
| `src/OmniAssert.Generator.Tests` | Generator / lowering compile tests. |
| `src/samples/VerifyInterceptorsSample` | Minimal app showing interceptor MSBuild wiring. |

### Build, test, and coverage

Solution file: `src/OmniAssert.slnx`.

```powershell
dotnet build src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx /p:CollectCoverage=true
```

CI (`.github/workflows/build-and-test.yml`) runs restore, Release build, tests, and uploads Coverlet Cobertura to Codecov from the `src` directory.

### Conventions

Use test names like `MethodUnderTest_Scenario_ExpectedBehavior` (for example, `Dispose_WhenNoFailures_ShouldNotThrow`).

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
