# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

OmniAssert is a lightweight, fluent assertion library for .NET focused on clear APIs and rich failure messages. It combines `[CallerArgumentExpression]`-backed `Verify` overloads with optional Roslyn-generated **C# interceptors** (.NET 10) for `Verify(bool)`, so simple conditions can use the same fluent path as `VerifyBool` while compound expressions still record the source text. Failures throw **`OmniAssertionException`**, which carries an **`AssertionCapture`** (source expression and optional operand dictionary) for structured diagnostics.

## Key features

- **Fluent `Verify`**: `Verify(subject).To…` for primitives, strings, collections, enums, nullables, `object`, dates, and more.
- **`Verify(bool)` interceptors** (optional): source generator plus `InterceptorsNamespaces`; simple identifiers are lowered to `VerifyBool(…).ToBeTrue()`, other boolean expressions call `VerifyBoolean` with the caller expression string (see below).
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

`OmniAssert.Generator` is a Roslyn analyzer (`IsRoslynComponent`); it is not shipped as a separate NuGet in this repo. Add a **project reference** with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"`, then set the MSBuild properties in the next section.

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

#### Boolean conditions: `Verify(bool)`, `VerifyBool`, and `VerifyBoolean`

- **`VerifyBool(condition)`** returns **`BoolAssertions`** for **`ToBeTrue()`** / **`ToBeFalse()`**, with the caller expression in failure output.
- **`Verify(condition)`** for `bool` builds an **`AssertionCapture`** from **`[CallerArgumentExpression]`** and calls **`VerifyBoolean`**.
- **`VerifyBoolean(bool, in AssertionCapture)`** is the structured entry point (marked **EditorBrowsable(Never)** on **`Assert`**). Use it when generated or hand-built captures include **`CapturedValues`** for operand-style context in **`OmniAssertionException`**.

With **interceptors enabled**, the generator replaces `Verify(bool)` call sites:

| Condition shape | Redirect |
|-----------------|----------|
| Bare identifier (including parenthesized) | `VerifyBool(condition, expression).ToBeTrue()` |
| Anything else (operators, literals, `!flag`, etc.) | `VerifyBoolean(condition, new AssertionCapture(expression, null))` |

So compound failures include the **expression string** on **`OmniAssertionException.SourceExpression`**; **`CapturedValues`** from interceptors is **null** for those paths because C# evaluates arguments before interception, so the generator cannot safely inline **`VerifyExpansionEngine`** lowering that references caller locals. The lowering implementation in **`OmniAssert.Generator.Rewrite.VerifyExpansionEngine`** remains available for **tooling and tests** that emit a full statement block in the original scope.

### Enabling boolean `Verify` interceptors

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

- **`OmniAssertEnableVerifyInterceptors`**: when `true`, the generator emits one file-scoped class per syntax tree under **`OmniAssert.Generated`**, with **`InterceptsLocationAttribute`** stubs for each interceptable `Assert.Verify(bool, …)` site. When `false` or omitted, nothing is emitted and **`Verify(bool)`** uses the normal **`[CallerArgumentExpression]`** path only.
- **`InterceptorsNamespaces`**: must include **`OmniAssert.Generated`** so the compiler may apply emitted interceptors.

---

## Contributor guide

### Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime: `Assert` entry points, assertion types, diffing, scopes, **`OmniAssertionException`**, **`AssertionCapture`**. |
| `src/OmniAssert.Generator` | Incremental source generator for boolean `Verify` interceptors. |
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
