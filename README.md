# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

OmniAssert is a lightweight, fluent assertion library for .NET focused on clear APIs and rich failure messages. Most checks use **`Verify(subject).To…`** with `[CallerArgumentExpression]` on the subject. For booleans you can use either **`Verify(flag).ToBeTrue()`** (fluent **`BoolAssertions`**) or **`VerifyExpression(condition)`** (immediate check, same failure shape as other structured boolean captures). Optional Roslyn **C# interceptors** (.NET 10) apply only to **`VerifyExpression(bool)`**. Failures throw **`OmniAssertionException`** with an **`AssertionCapture`** (source expression and optional operand dictionary) where applicable.

## Key features

- **Fluent `Verify`**: `Verify(subject).To…` for primitives (including **`bool`** via **`BoolAssertions`**), strings, collections, enums, nullables, `object`, dates, and more.
- **`VerifyExpression(bool)` interceptors** (optional): source generator plus `InterceptorsNamespaces`; simple identifiers are lowered to `Verify(…).ToBeTrue()`, other boolean expressions call **`VerifyBoolean`** with the caller expression string (see [Enabling boolean `VerifyExpression` interceptors](#enabling-boolean-verifyexpression-interceptors)).
- **Soft assertions**: `AssertionScope` collects multiple `OmniAssertionException` instances and throws once when the scope is disposed.
- **Deep equivalence**: `Verify(actual).ToBeEquivalentTo(expected)` walks public properties and reports a structured diff.
- **Terminal-friendly output**: messages use ANSI coloring where appropriate.
- **Async helpers**: `ThrowsAsync` / `NotThrowAsync` and `CompleteWithin` for task-based code paths.

---

## Consumer guide

### Installation

**Runtime**

The NuGet package ID is **`OmniAssert`** (assembly `OmniAssert.Core`). When you consume a pre-built package:

```xml
<ItemGroup>
  <PackageReference Include="OmniAssert" Version="1.0.0-alpha.0" />
</ItemGroup>
```

From this repository, reference `src/OmniAssert.Core/OmniAssert.Core.csproj` instead.

**Boolean interceptors (analyzer)**

`OmniAssert.Generator` is a Roslyn analyzer (`IsRoslynComponent`); it is not shipped as a separate NuGet in this repo. Add a **project reference** with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"`, then set the MSBuild properties under [Enabling boolean `VerifyExpression` interceptors](#enabling-boolean-verifyexpression-interceptors).

### Getting started

```csharp
using static OmniAssert.Assert;
```

### Basic assertions

#### Numeric

Assertions use **`INumber<T>`** via `NumericAssertions<T>`. Overloads exist for the built-in numeric types and **`BigInteger`**.

```csharp
Verify(42).ToBe(42);
Verify(10).ToBeGreaterThan(5);
Verify(3.14159).ToBeApproximately(3.14, 0.01);
Verify(age).ToBeInRange(18, 99);
```

#### Strings

```csharp
Verify("Hello World").ToContain("Hello");
Verify("filename.txt").ToEndWith(".txt");
Verify(email).ToMatch(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
Verify("test").ToBe("TEST", StringComparison.OrdinalIgnoreCase);
```

#### Collections

```csharp
Verify(users).ToContain(currentUser);
Verify(results).HasCount(3);
Verify(list).AllSatisfy(x => x > 0);
Verify(actualList).ToBeEquivalentTo(expectedList); // unordered comparison
```

#### Nullability

```csharp
VerifyNullable(someObject).NotToBeNull();
Verify(someString).ToBeNullOrWhiteSpace();
```

#### Enums

```csharp
Verify(MyEnum.On).ToBe(MyEnum.On);
```

#### Booleans

**Fluent** — `Verify(bool)` returns `BoolAssertions`. You must call `ToBeTrue()` or `ToBeFalse()`; failures use subject-style messages (expression string plus colored actual value).

```csharp
Verify(isValid).ToBeTrue();
Verify(featureFlag).ToBeFalse();
```

**Immediate** — `VerifyExpression(bool)` asserts in one call (void). On failure it throws `OmniAssertionException` built from `AssertionCapture` / `ForBooleanFailure` (expression line and optional captured operands when supplied by tooling or `VerifyBoolean`).

```csharp
VerifyExpression(x > 0 && count < limit);
```

Use **`Verify(…).ToBeTrue()`** when you want the same fluent style as `Verify(42).ToBe(42)`. Use **`VerifyExpression(…)`** when a single call reads better, or when you [enable interceptors](#enabling-boolean-verifyexpression-interceptors) so compound expressions still get structured handling at the call site.

#### Objects

```csharp
Verify(instance).ToBeOfType<MyClass>();
Verify(instance).ToBeAssignableTo<IMyInterface>();
```

#### Exceptions and async behavior

```csharp
Throws<ArgumentException>(() => someObject.DoWork());
await ThrowsAsync<InvalidOperationException>(async () => await someTask);

NotThrow(() => { /* must not throw */ });
await NotThrowAsync(async () => await someTask);

await CompleteWithin(TimeSpan.FromSeconds(1), async () => await WorkAsync());

// Further checks on the caught exception
Throws<Exception>(() => action()).WithMessage("Error").WithInnerException<SocketException>();
```

#### Dates

```csharp
Verify(dateTime).ToBeAfter(DateTime.Now.AddDays(-1));
Verify(dateTimeOffset).ToBeWithin(TimeSpan.FromMinutes(1), referenceInstant);
```

### Advanced usage

#### Assertion scopes (soft assertions)

```csharp
using (new AssertionScope())
{
    Verify(user.Name).ToBe("John");
    Verify(user.Age).ToBe(30);
}
```

#### Object equivalence

Deep comparison is on **`ObjectAssertions`** (not a standalone `VerifyEquivalent` helper):

```csharp
Verify(actualDto).ToBeEquivalentTo(expectedDto);
```

Boolean APIs in more detail: see [Booleans](#booleans) under *Basic assertions*. Interceptor lowering and **`VerifyBoolean`**: see the next section.

### Enabling boolean `VerifyExpression` interceptors

Use the **.NET 10** SDK and C# **14**. Configure the compiler and analyzer as follows (see **`src/samples/VerifyInterceptorsSample/VerifyInterceptorsSample.csproj`** for a working project):

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

- **`OmniAssertEnableVerifyInterceptors`**: when `true`, the generator emits one file-scoped class per syntax tree under **`OmniAssert.Generated`**, with **`InterceptsLocationAttribute`** stubs for each interceptable **`Assert.VerifyExpression(bool, …)`** site. When `false` or omitted, nothing is emitted and **`VerifyExpression(bool)`** uses the normal **`[CallerArgumentExpression]`** path only.
- **`InterceptorsNamespaces`**: must include **`OmniAssert.Generated`** so the compiler may apply emitted interceptors.

**What gets rewritten:** only **`VerifyExpression`** call sites are intercepted (not plain **`Verify(bool)`** fluent calls).

| Condition shape at the call site | Interceptor redirect |
|-----------------------------------|----------------------|
| Bare identifier (including parenthesized) | `Verify(condition, expression).ToBeTrue()` |
| Anything else (operators, literals, `!flag`, compound logic, …) | `VerifyBoolean(condition, new AssertionCapture(expression, null))` |

**`VerifyBoolean(bool, in AssertionCapture)`** is **`[EditorBrowsable(Never)]`**: the public surface is **`VerifyExpression`** or hand-built captures for tooling. **`CapturedValues`** on the capture is populated when callers (or **`VerifyExpansionEngine`** output) supply a dictionary; emitted interceptors use **`null`** for compound expressions because arguments are evaluated before interception, so the generator cannot safely inline lowering that references caller locals. **`OmniAssert.Generator.Rewrite.VerifyExpansionEngine`** still lowers boolean trees for **tests and external tooling** that emit a full block in the original scope.

---

## Contributor guide

### Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime: `Assert` entry points, assertion types, diffing, scopes, **`OmniAssertionException`**, **`AssertionCapture`**. |
| `src/OmniAssert.Generator` | Incremental source generator for **`VerifyExpression(bool)`** interceptors. |
| `src/OmniAssert.Generator/Rewrite` | **`VerifyExpansionEngine`**: lowers boolean expressions for tooling/tests (not inlined into interceptors for arbitrary local-capturing expressions). |
| `src/OmniAssert.Tests` | Main tests (interceptors **enabled**). |
| `src/OmniAssert.Tests.NoInterceptors` | Smoke tests with interceptors **disabled**. |
| `src/OmniAssert.Generator.Tests` | Generator and lowering tests. |
| `src/samples/VerifyInterceptorsSample` | Minimal executable sample included in the solution. |

### Build and test

Projects target **.NET 10** (solution file: **`src/OmniAssert.slnx`**).

```bash
dotnet build src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx /p:CollectCoverage=true
```

CI (**`.github/workflows/build-and-test.yml`**) runs restore, Release build, and tests with Coverlet Cobertura output from the **`src`** directory.

### Test naming

Use **`MethodUnderTest_Scenario_ExpectedBehavior`** (for example, `Dispose_WhenNoFailures_ShouldNotThrow`).

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
