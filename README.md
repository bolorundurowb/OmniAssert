# OmniAssert

OmniAssert is a small assertion library for .NET that combines **fluent checks** on values (`Verify(x).ToBe(expected)`) with **richer failures** for boolean conditions when you enable an MSBuild step that rewrites `Verify(bool)` call sites to capture operand values at compile time.

## What you get

- **Fluent assertions** for common types (for example `int`, `string`, `IEnumerable<T>`) with expression names in messages via `[CallerArgumentExpression]`.
- **Boolean rewrite** (optional): `Verify(a == b && c > 0)` can fail with captured values and the original expression, not only “expected true”.
- **`AssertionScope`**: collect multiple failures and throw when the scope ends (soft assertions).
- **`VerifyEquivalent`**: compare two objects by public properties and emit a structured diff on mismatch.

## Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime API: `Assert.Verify`, assertion types, `AssertionScope`, `ObjectDiffWalker`, and related types. |
| `src/OmniAssert.Build` | MSBuild task and targets that rewrite `Verify(bool)` call sites before `CoreCompile` so failures can include captured values. |
| `src/OmniAssert.Generator` | Roslyn incremental generator (placeholder for future compile-time features; boolean rewriting is handled by the Build task). |
| `tests/OmniAssert.Tests` | xUnit tests for Core behavior and rewriter-related APIs. The test project references the generator as an analyzer only (`ReferenceOutputAssembly="false"`). |

There is no solution (`.sln`) file in the repository; build and test using the project paths below.

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download) (projects target `net10.0`; the generator targets `netstandard2.0`)

## Build

```powershell
dotnet build "src/OmniAssert.Core/OmniAssert.Core.csproj"
dotnet build "src/OmniAssert.Build/OmniAssert.Build.csproj"
dotnet build "src/OmniAssert.Generator/OmniAssert.Generator.csproj"
```

Build Core before Build if you are wiring the task from a local checkout (the rewriter resolves `OmniAssert.Core` for semantic analysis).

## Using OmniAssert in a project

1. Add a project reference to `OmniAssert.Core`.
2. Reference `OmniAssert.Build` **without** referencing its assembly (so the task can load without conflicting with the app):

   ```xml
   <ProjectReference Include="path\to\OmniAssert.Build.csproj" ReferenceOutputAssembly="false" />
   ```

3. Import the targets (adjust the path to match your layout):

   ```xml
   <Import Project="path\to\OmniAssert.Build\build\OmniAssert.Build.targets" />
   ```

4. Enable rewriting (set explicitly if you do not rely on defaults from your own props):

   ```xml
   <PropertyGroup>
     <OmniAssertRewrite>true</OmniAssertRewrite>
   </PropertyGroup>
   ```

The targets copy task dependencies from `$(OmniAssertBuildTaskSourceDir)` and pass `$(OmniAssertCoreAssembly)` into the rewriter. Point those at your built `OmniAssert.Build` output folder and `OmniAssert.Core.dll` respectively (for this repo, `Directory.Build.props` sets defaults under `src/.../bin/$(Configuration)/net10.0/` after a local build).

Set `OmniAssertRewrite` to `false` in test projects if the test host would lock the MSBuild task assembly; you can still exercise Core APIs and non-rewritten boolean checks.

## API sketch

```csharp
using static OmniAssert.Assert;

Verify(42).ToBe(42);
Verify("hello").ToContain("ell");
Verify(new[] { 1, 2, 3 }).ToContain(2);
VerifyBool(flag).ToBeTrue();

// With rewrite: failure can include captured operands and the source expression
Verify(x > 0 && y < 100);

VerifyEquivalent(expectedDto, actualDto);

using (new AssertionScope())
{
    Verify(1).ToBe(1);
    Verify(2).ToBe(3); // Collected; thrown on scope dispose
}
```

## Tests

```powershell
dotnet test "tests/OmniAssert.Tests/OmniAssert.Tests.csproj"
```

### Test naming

Tests follow **MethodUnderTest_Scenario_ExpectedBehavior** (PascalCase segments separated by underscores), for example `Dispose_WhenNoFailures_ShouldNotThrow`.

## Code coverage

Coverage uses [Coverlet](https://github.com/coverlet-coverage/coverlet). The test project references `coverlet.collector` (for `dotnet test --collect:"XPlat Code Coverage"`) and `coverlet.msbuild` (for MSBuild-driven reports).

### MSBuild (summary table + Cobertura XML)

```powershell
dotnet test "tests/OmniAssert.Tests/OmniAssert.Tests.csproj" `
  /p:CollectCoverage=true `
  /p:CoverletOutputFormat=cobertura `
  /p:CoverletOutput="$PWD/TestResults/coverage/"
```

This instruments **OmniAssert.Core**. The console prints line, branch, and method coverage; `coverage.cobertura.xml` is written under `TestResults/coverage/`.

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
| OmniAssert.Core | 68.2% | 54.65% | 91.66% |

Coverage does not include the MSBuild rewriter (`OmniAssert.Build`); that code is validated by building consumer-style projects with `OmniAssertRewrite` enabled.

## License

This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
