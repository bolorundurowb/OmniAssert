# OmniAssert

OmniAssert is a small assertion library for .NET with two main ideas: **fluent checks** on values (`Verify(x).ToBe(expected)`) and **richer failures** for boolean expressions when you opt into a compile-time rewrite that captures operand values.

## Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime API: `Assert.Verify`, assertion types, `AssertionScope`, `ObjectDiffWalker`, and related types. |
| `src/OmniAssert.Build` | MSBuild task and targets that rewrite `Verify(bool)` call sites before `CoreCompile` so failures can include captured values. |
| `src/OmniAssert.Generator` | Roslyn incremental generator project (currently a no-op placeholder; rewriting is owned by the Build task). |
| `tests/OmniAssert.Tests` | xUnit tests for Core behavior and rewriter-related APIs. |
| `samples/RewriteSample` | Example console app with `OmniAssertRewrite` enabled. |

There is no solution (`.sln`) file yet; build and test using the project paths shown below.

## Requirements

- [.NET 8 SDK](https://dotnet.microsoft.com/download)

## Build

```powershell
dotnet build "src/OmniAssert.Core/OmniAssert.Core.csproj"
dotnet build "src/OmniAssert.Build/OmniAssert.Build.csproj"
dotnet build "samples/RewriteSample/RewriteSample.csproj"
```

## Using OmniAssert in a project

1. Add a project reference to `OmniAssert.Core`.
2. Reference `OmniAssert.Build` **without** referencing its assembly (so the task can load without conflicting with the app):

   ```xml
   <ProjectReference Include="path\to\OmniAssert.Build.csproj" ReferenceOutputAssembly="false" />
   ```

3. Import the targets (adjust the relative path to match your repo):

   ```xml
   <Import Project="path\to\OmniAssert.Build\build\OmniAssert.Build.targets" />
   ```

4. Enable rewriting (defaults follow your targets; the sample sets it explicitly):

   ```xml
   <PropertyGroup>
     <OmniAssertRewrite>true</OmniAssertRewrite>
   </PropertyGroup>
   ```

The test project sets `OmniAssertRewrite` to `false` so the MSBuild task assembly is not locked by the test host while still exercising Core APIs.

## API sketch

```csharp
using static OmniAssert.Assert;

Verify(42).ToBe(42);
Verify("hello").ToContain("ell");
Verify(x > 0 && y < 100); // With rewrite: failure can include captured x, y, and the source expression

using (new AssertionScope())
{
    Verify(1).ToBe(1);
    Verify(2).ToBe(3); // Collected; thrown on scope dispose
}
```

## Tests

Run all tests:

```powershell
dotnet test "tests/OmniAssert.Tests/OmniAssert.Tests.csproj"
```

### Test naming

Tests follow the common C# style **MethodUnderTest_Scenario_ExpectedBehavior** (PascalCase segments separated by underscores), for example `Dispose_WhenNoFailures_ShouldNotThrow`.

## Code coverage

Coverage is collected with [Coverlet](https://github.com/coverlet-coverage/coverlet). The test project references `coverlet.collector` (for `dotnet test --collect:"XPlat Code Coverage"`) and `coverlet.msbuild` (for MSBuild-driven reports).

### MSBuild (summary table + Cobertura XML)

```powershell
dotnet test "tests/OmniAssert.Tests/OmniAssert.Tests.csproj" `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput="$PWD/TestResults/coverage/"
```

This instruments **`OmniAssert.Core`** (the assembly under test). The console prints line, branch, and method coverage; `coverage.cobertura.xml` is written under `TestResults/coverage/`.

### VSTest collector (XML under `TestResults`)

```powershell
dotnet test "tests/OmniAssert.Tests/OmniAssert.Tests.csproj" `
  --collect:"XPlat Code Coverage" `
  --results-directory "./TestResults"
```

### Measured snapshot (illustrative)

The following figures were produced by the MSBuild command above on this repository; re-run the command for current numbers:

| Module | Line | Branch | Method |
|--------|------|--------|--------|
| OmniAssert.Core | 69.21% | 54.65% | 91.93% |

Coverage does not yet include the MSBuild rewriter (`OmniAssert.Build`); that code is validated via integration-style builds (for example `RewriteSample`) rather than the unit test project.
