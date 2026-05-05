# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

---


> **Provenance:** OmniAssert was built with **substantial help from AI-assisted coding tools**. If you are evaluating it for production, security-sensitive, or compliance-heavy use, apply the same scrutiny you would to any rapidly authored codebase: read critical paths, run your own tests, and satisfy your organisation’s review and supply-chain policies.

**OmniAssert** is on NuGet as **[`OmniAssert`](https://www.nuget.org/packages/OmniAssert)**. The first line is **`1.0.0-alpha.0`**: a small runtime (`OmniAssert.Core`) plus an optional Roslyn generator shipped inside the same package.

### Why this package exists

OmniAssert brings together ideas that are scattered across popular .NET assertion stacks: **fluent APIs that read in natural language** (`Verify(subject).To…`), **rich exception text and tidy failure output** (including caller expression names via `[CallerArgumentExpression]`), a **deep object graph diff** behind `ToBeEquivalentTo` (public properties, sequences, cycle-safe walks), **`AssertionScope`** for soft asserts that aggregate many failures into one report, and—when you opt in—**compile-time support for complex boolean checks** so failures are easier to diagnose (`VerifyExpression` interceptors for full expression text; an advanced **rewrite** mode can record **intermediate sub-values** when a compound condition fails).

If you use `AssertionScope`, failures are collected and reported together when the scope is disposed.

### Requirements

- **Runtime**: projects targeting **.NET 10** (or a compatible TFM) can reference the **`OmniAssert`** NuGet package (runtime assembly **`OmniAssert.Core`**) or reference `OmniAssert.Core` from this repository.
- **Optional interceptors**: **.NET 10 SDK**, **C# 14**, and the MSBuild properties below. Interceptors only affect `VerifyExpression(bool, string?)` (the `string` is normally supplied by `[CallerArgumentExpression]`).

### Quick start

1. Add the package (or a project reference to `src/OmniAssert.Core/OmniAssert.Core.csproj`).
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

### Install

**From NuGet** (package id `OmniAssert`; assembly name `OmniAssert.Core`):

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="1.0.0-alpha.0" />
</ItemGroup>
```

**From this repository**: reference `src/OmniAssert.Core/OmniAssert.Core.csproj`. The same Roslyn generator that ships in the NuGet package lives under `analyzers/dotnet/cs`; to enable it from a fork, add an analyzer reference to `src/OmniAssert.Generator/OmniAssert.Generator.csproj` (see `src/samples/VerifyInterceptorsSample/VerifyInterceptorsSample.csproj`).

### Public API (what you reference in tests)

Consumers rely on **`Assert`** (usually with `using static OmniAssert.Assert`), the fluent assertion structs it returns, **`AssertionScope`**, **`OmniAssertionException`**, and **`AssertionCapture`** (for advanced `VerifyExpression` scenarios). Types such as the diff engine and terminal colour helpers are **internal implementation details**—you exercise them through **`Verify(…).ToBeEquivalentTo(…)`** and other public assertions. Optional generator output appears under **`OmniAssert.Generated`** when interceptors are enabled.

### Core usage

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

### API at a glance

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

### Common scenarios

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

### Optional: richer boolean failures with interceptors

Interceptors are **off** by default. When enabled, the `OmniAssert.Generator` analyzer emits stubs under `OmniAssert.Generated` so `VerifyExpression(bool, …)` call sites can be rewritten at compile time:

- a bare boolean identifier (including parenthesised) becomes `Verify(condition, expression).ToBeTrue()`;
- any other boolean shape becomes `VerifyExpression(condition, expression)` with the captured expression text (same failure path as the non-intercepted API).

**NuGet consumers** — the `OmniAssert` package already includes the generator as a Roslyn analyzer. Enable interceptors with MSBuild only:

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
  <PackageReference Include="OmniAssert" Version="1.0.0-alpha.0" />
</ItemGroup>
```

**Building from this repository** — reference the runtime and attach the generator to your project’s compilation (same wiring as `src/samples/VerifyInterceptorsSample/VerifyInterceptorsSample.csproj`):

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
  <ProjectReference Include="path/to/OmniAssert.Core/OmniAssert.Core.csproj" />
  <ProjectReference Include="path/to/OmniAssert.Generator/OmniAssert.Generator.csproj"
                    OutputItemType="Analyzer"
                    ReferenceOutputAssembly="false" />
</ItemGroup>
```

- **`OmniAssertEnableVerifyInterceptors`**: `true` emits interceptors per syntax tree; `false` (or omitted) keeps `VerifyExpression` on the normal `[CallerArgumentExpression]` path.
- **`InterceptorsNamespaces`**: must include `OmniAssert.Generated` so the compiler applies emitted interceptors.

Only `VerifyExpression(bool, string?)` call sites are intercepted (not `Verify(bool).ToBeTrue()` fluent calls). A separate overload, `VerifyExpression(bool, AssertionCapture)`, is used by boolean lowering in tooling (`VerifyExpansionEngine`); it is **not** intercepted.

| Condition at the call site | What the interceptor does |
|----------------------------|-----------------------------|
| Bare identifier (including parenthesised) | `Verify(condition, expression).ToBeTrue()` |
| Operators, literals, `!flag`, compound logical expressions, … | `VerifyExpression(condition, expression)` |

### Optional: operand capture via source rewrite (advanced)

For **repository builds** only, you can set **`OmniAssertEnableRewrite`** to `true` and list a `.cs` file as an **`AdditionalFiles`** item (and exclude that file from **`Compile`**). The generator then emits a rewritten compilation unit that lowers `VerifyExpression` to `VerifyExpression(bool, AssertionCapture)` with per-subexpression values in the failure message. See `src/samples/VerifyInterceptorsSample` for a minimal example. This is not required for normal package consumption.

### More examples

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

This section is for people building or changing OmniAssert. If you only consume the package, you can stop after the consumer section.

The codebase was bootstrapped with **significant AI assistance**; treat contributions like any other open-source change (clear intent, tests, and review).

### Contribution workflow

1. Fork and clone the repository.
2. Build and run tests locally.
3. Make focused changes with tests.
4. Open a PR with a clear description of behaviour changes.

### Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime: `Assert`, fluent assertion structs, `AssertionScope`, `OmniAssertionException`, `AssertionCapture`, plus internal diff/colour helpers. |
| `src/OmniAssert.Generator` | Roslyn incremental generator: `VerifyExpression(bool, string?)` interceptors when `OmniAssertEnableVerifyInterceptors` is `true`; optional rewrite when `OmniAssertEnableRewrite` is `true`. |
| `src/OmniAssert.Generator/Rewrite` | `VerifyExpansionEngine`: lowers boolean trees to `VerifyExpression(bool, AssertionCapture)` for tests and tooling. |
| `src/OmniAssert.Tests` | Main tests (interceptors enabled). |
| `src/OmniAssert.Tests.NoInterceptors` | Smoke tests (interceptors disabled). |
| `src/OmniAssert.Generator.Tests` | Generator / lowering compile tests. |
| `src/samples/VerifyInterceptorsSample` | Minimal app showing interceptor and rewrite MSBuild wiring. |

### Build, test, and coverage

Solution file: `src/OmniAssert.slnx`.

```powershell
dotnet build src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx /p:CollectCoverage=true
```

CI (`.github/workflows/build-and-test.yml`) runs restore, a Release build, tests, and uploads Coverlet Cobertura to Codecov from the `src` directory.

### Conventions

Use test names like `MethodUnderTest_Scenario_ExpectedBehaviour` (for example, `Dispose_WhenNoFailures_ShouldNotThrow`).

---

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
