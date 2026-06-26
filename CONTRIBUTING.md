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
| `src/OmniAssert.Extensions` | Domain-specific fluent assertions for web, financial, security, and regional validation — companion package (`OmniAssert.Extensions` on NuGet). |
| `src/OmniAssert.Generator` | Roslyn incremental generator: `Ensure.Expression(bool, string?)` and legacy `VerifyExpression` interceptors (on by default, opt out with `OmniAssertDisableVerifyInterceptors`); optional operand-capture rewrite via `AdditionalFiles`. |
| `src/OmniAssert.Generator/Rewrite` | `VerifyExpansionEngine`: lowers boolean trees to `Expression(bool, AssertionCapture)` for tests and tooling. |
| `src/tests/OmniAssert.Tests` | Main tests (interceptors enabled). |
| `src/tests/OmniAssert.Extensions.Tests` | Tests for the Extensions package. |
| `src/tests/OmniAssert.Generator.Tests` | Generator and lowering compile tests. |
| `src/samples/VerifyInterceptorsSample` | Minimal project showing interceptor and rewrite MSBuild wiring. |

## Documentation site (gh-pages)

The public docs at [bolorundurowb.github.io/OmniAssert](https://bolorundurowb.github.io/OmniAssert/) are served from the **`gh-pages`** branch. Many contributors keep a separate local checkout (for example `OmniAssert-gh-pages` beside this repo) and push `index.html` updates there:

```powershell
cd path\to\OmniAssert-gh-pages
git add index.html
git commit -m "Document API changes"
git push origin gh-pages
```

Site files: `index.html`, `assets/omni-assert-logo.svg`, `assets/omni-assert-logo.png`.

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

- **`OmniAssertIncrementalGenerator`**: interceptor discovery, `GetInterceptableLocation`, generated `[InterceptsLocation]` stubs, and the **operand-capture rewrite** path via `AdditionalFiles` are only exercised indirectly (sample app and manual builds), not by focused unit tests.
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

## Advanced: operand capture via source rewrite

This path is intended for **advanced setups** that supply a `.cs` file as an **`AdditionalFiles`** item and exclude that file from **`Compile`**.

The generator detects the `AdditionalFiles` entry automatically (no extra MSBuild property required) and emits a rewritten compilation unit that lowers legacy `VerifyExpression` call sites to `Ensure.Expression(bool, AssertionCapture)` with per-subexpression values in the failure message. It is **not** required for normal NuGet consumption. Wiring details and a sample live in **`src/samples/VerifyInterceptorsSample`**.
