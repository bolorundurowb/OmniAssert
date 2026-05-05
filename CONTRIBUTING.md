# Contributing to OmniAssert

This document is for people who build, test, or change the OmniAssert repository. If you only use the **[`OmniAssert`](https://www.nuget.org/packages/OmniAssert)** package, see the [README](README.md) instead.

The codebase was bootstrapped with **significant AI assistance**; treat contributions like any other open-source change (clear intent, tests, and review).

## Workflow

1. Fork and clone the repository.
2. Build and run tests locally (see below).
3. Make focused changes with tests.
4. Open a pull request with a clear description of behaviour changes.

## Repository layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime: `Assert`, fluent assertion structs, `AssertionScope`, `OmniAssertionException`, `AssertionCapture`, plus internal diff and colour helpers. |
| `src/OmniAssert.Generator` | Roslyn incremental generator: `VerifyExpression(bool, string?)` interceptors when `OmniAssertEnableVerifyInterceptors` is `true`; optional rewrite when `OmniAssertEnableRewrite` is `true`. |
| `src/OmniAssert.Generator/Rewrite` | `VerifyExpansionEngine`: lowers boolean trees to `VerifyExpression(bool, AssertionCapture)` for tests and tooling. |
| `src/tests/OmniAssert.Tests` | Main tests (interceptors enabled). |
| `src/tests/OmniAssert.Generator.Tests` | Generator and lowering compile tests. |
| `src/samples/VerifyInterceptorsSample` | Minimal project showing interceptor and rewrite MSBuild wiring. |

## Build, test, and coverage

Solution file: `src/OmniAssert.slnx`.

```powershell
dotnet build src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx
dotnet test src/OmniAssert.slnx /p:CollectCoverage=true
```

With `CollectCoverage=true`, Coverlet writes **one Cobertura file per test project** under `src/coverage/` (for example `src/coverage/OmniAssert.Tests.cobertura.xml` and `src/coverage/OmniAssert.Generator.Tests.cobertura.xml`). Shared package and assembly versions live in **`src/Directory.Build.props`**; test-only Coverlet settings are in **`src/Directory.Build.targets`** so `IsTestProject` is evaluated correctly.

CI (`.github/workflows/build-and-test.yml`) runs restore, a Release build, tests with coverage for all solution test projects, and uploads Cobertura reports from `src/coverage/` to Codecov.

## Test coverage gaps (prioritised)

**High value**

- **`OmniAssertIncrementalGenerator`**: interceptor discovery, `GetInterceptableLocation`, generated `[InterceptsLocation]` stubs, and the **rewrite** path driven by `OmniAssertEnableRewrite` and additional files are only exercised indirectly (sample app and manual builds), not by focused unit tests.
- **`VerifyExpansionEngine`**: more boolean shapes (short-circuit edge cases, additional binary operators, parenthesised and nested forms) than the current compile tests cover.

**Medium**

- **`VerificationFlow`** and other glue in Core used by generated or interceptor paths.
- **Core assertion APIs** with uneven coverage (for example parts of `ObjectAssertions`, `CollectionAssertions`, `NumericAssertions`, `DateTimeAssertions`) beyond what `ExtendedAssertionTests` and `AdditionalCoverageTests` already hit.

**Lower**

- **`AnsiColour`**, formatting-only branches in `OmniAssertionException`, and defensive paths in `ObjectDiffWalker` once the main diff scenarios are covered.

## Conventions

Use test names like `MethodUnderTest_Scenario_ExpectedBehaviour` (for example, `Dispose_WhenNoFailures_ShouldNotThrow`).

## Project references instead of NuGet

When wiring **OmniAssert.Core** and **OmniAssert.Generator** from this repository (or a fork), mirror the sample project:

- `src/samples/VerifyInterceptorsSample/VerifyInterceptorsSample.csproj`

Use a `ProjectReference` to the generator with `OutputItemType="Analyzer"` and `ReferenceOutputAssembly="false"`, plus the same MSBuild properties as in the README’s interceptor section.
