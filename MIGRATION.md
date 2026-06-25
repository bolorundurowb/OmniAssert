# Migrating from OmniAssert v1 to v2

OmniAssert v2 modernises the fluent API, avoids naming clashes with other test frameworks (such as NUnit's `Assert`), and ships Roslyn analyzers to automate migration.

Legacy v1 syntax **still compiles** in v2 but is marked `[Obsolete]` and surfaces compiler warnings.

## Deprecation timeline

The following **will be removed in OmniAssert v3.0** (the next major release after v2):

- The **`Assert`** static class and all **`Assert.*`** entry points
- The **`.Verify()`** fluent extension (replaced by **`.Must()`**)
- All legacy **`To*`** / **`NotTo*`** assertion methods (replaced by **`Be*`** / **`Not*`** / **`Have*`** / etc.)
- The **`Expect`** compatibility wrapper (use **`Ensure`** instead)
- Extension **`VerifyExpression(...)`** (replaced by static **`Ensure.Expression(...)`**)

There is no planned removal date for **`VerifyNullable`**, exception extensions, file/directory helpers, or other APIs that were not renamed in v2.

**Note:** Extension **`VerifyExpression(...)`** remains fully functional in v2 (warning only) but is deprecated; use static **`Ensure.Expression(...)`** for new code.

Migrate to v2 syntax before upgrading to v3. After v3, code that still uses deprecated members will fail to compile.

## Summary of changes

| v1 (legacy)                                   | v2 (recommended)                               |
|-----------------------------------------------|------------------------------------------------|
| `Assert` static entry point                   | `Ensure`                                       |
| `.Verify()`                                   | `.Must()`                                      |
| `.ToBeTrue()`, `.ToBe(42)`, `.ToContain("x")` | `.BeTrue()`, `.Be(42)`, `.Contain("x")`        |
| `.NotToBe(...)`, `.NotToBeNull()`             | `.NotBe(...)`, `.NotBeNull()`                  |
| `.ToHaveCount(3)`, `.ToBeGreaterThan(0)`      | `.HaveCount(3)`, `.BeGreaterThan(0)`           |
| `Expect.Throws<T>(...)` (optional)            | `Ensure.Throws<T>(...)` or `(...).Throws<T>()` |
| `(condition).VerifyExpression()`              | `Ensure.Expression(condition)`                 |
| `Assert.VerifyExpression(condition)`          | `Ensure.Expression(condition)`                 |

Methods that never used a `To` prefix in v1 are unchanged—for example `ContainKey`, `HaveHost`, `AllSatisfy`, and file helpers like `HaveContent`.

## Entry point: `Assert` → `Ensure`

v1 used a static class named `Assert`, which conflicts with NUnit, xUnit, and other frameworks.

```csharp
// v1
using OmniAssert;
OmniAssert.Assert.VerifyExpression(condition);

// v2 (legacy extension — still works with warnings)
Ensure.VerifyExpression(condition);

// v2 (recommended)
Ensure.Expression(condition);
```

Extension-based assertions do not require a class prefix:

```csharp
// v1
user.Email.Verify().ToContain("@");

// v2
user.Email.Must().Contain("@");
```

## Fluent chain: `Verify()` → `Must()`

Replace every fluent entry call:

```csharp
// v1
count.Verify().ToBeGreaterThan(0);

// v2
count.Must().BeGreaterThan(0);
```

## Assertion grammar: drop the `To` prefix

v2 uses direct, sentence-like method names. The general rules are:

- `ToXxx(...)` → `Xxx(...)` — e.g. `ToBeFalse()` → `BeFalse()`, `ToHaveCount(3)` → `HaveCount(3)`
- `NotToXxx(...)` → `NotXxx(...)` — e.g. `NotToBeNull()` → `NotBeNull()`, `NotToContain(x)` → `NotContain(x)`

### Common mappings

| v1                                     | v2                                 |
|----------------------------------------|------------------------------------|
| `ToBeTrue()` / `ToBeFalse()`           | `BeTrue()` / `BeFalse()`           |
| `ToBe(expected)`                       | `Be(expected)`                     |
| `NotToBe(unexpected)`                  | `NotBe(unexpected)`                |
| `ToBeNull()` / `NotToBeNull()`         | `BeNull()` / `NotBeNull()`         |
| `ToBeEmpty()` / `NotToBeEmpty()`       | `BeEmpty()` / `NotBeEmpty()`       |
| `ToContain(...)` / `NotToContain(...)` | `Contain(...)` / `NotContain(...)` |
| `ToBeGreaterThan(...)`                 | `BeGreaterThan(...)`               |
| `ToBeEquivalentTo(...)`                | `BeEquivalentTo(...)`              |
| `ToBeOfType<T>()`                      | `BeOfType<T>()`                    |

Apply the same pattern to other `To*` / `NotTo*` methods on assertion types.

## What did not change

These APIs keep their v1 names in v2:

- **`VerifyNullable(...)`** — nullable reference/value entry point (e.g. `value.VerifyNullable().BeNull()`)
- **Exception extensions** — `(...).Throws<T>()`, `NotThrow()`, `ThrowsAsync<T>()`, etc. (now resolved via `Ensure`, not `Assert`)
- **File/directory helpers** — `"path".FileExists().HaveContent(...)`, `"dir".DirectoryExists().BeEmpty()`
- **`AssertionScope`** — soft assertions behave the same
- **`WithMessage` / `WithInnerException<T>`** — exception assertion chaining

## Booleans: `Must()` vs `Ensure.Expression()`

Both styles remain supported for boolean assertions:

```csharp
// Fluent (v2)
isValid.Must().BeTrue();

// Expression style (recommended for compound conditions)
Ensure.Expression(x > 0 && count < limit);
```

Legacy extension syntax still works with compiler and analyzer warnings:

```csharp
(x > 0 && count < limit).VerifyExpression(); // deprecated
```

When interceptors are enabled, a bare identifier such as `Ensure.Expression(flag)` is rewritten at compile time to `Ensure.Must(flag, expression).BeTrue()`. Legacy `(flag).VerifyExpression()` call sites are still intercepted and emit `Ensure.Expression` at runtime.

## Automated migration (Roslyn analyzers)

Starting in v2, the **`OmniAssert`** NuGet package includes **`OmniAssert.Analyzers.dll`** alongside the existing source generator. No extra package reference is required.

### Diagnostic rules

| ID        | Detects                                                       | Suggested fix                                |
|-----------|---------------------------------------------------------------|----------------------------------------------|
| **OA001** | Use of legacy `OmniAssert.Assert`                             | Replace with `Ensure`                        |
| **OA002** | `.Verify()` fluent entry                                      | Replace with `.Must()`                       |
| **OA003** | Legacy `To*` / `NotTo*` assertion methods after a fluent root | Replace with `Be*` / `Not*` / `Have*` / etc. |
| **OA004** | Legacy `VerifyExpression(...)` on `Ensure` or `Assert`        | Rewrite to **`Ensure.Expression(...)`**        |

### Applying fixes

In Visual Studio, Rider, or VS Code with C# Dev Kit:

1. Build the solution so analyzers load from the package.
2. Open a file with legacy syntax (light bulb / squiggle on OA001–OA004).
3. Choose **Migrate to Ensure/Must/Be* syntax**, or **Fix all occurrences in Solution** for batch migration.

Example transformations:

```csharp
Assert.Throws<InvalidOperationException>(() => act());
// → Ensure.Throws<InvalidOperationException>(() => act());

value.Verify().ToBeFalse();
// → value.Must().BeFalse();

items.Verify().NotToBeNull();
// → items.Must().NotBeNull();

(x > y).VerifyExpression();
// → Ensure.Expression(x > y);
```

## Framework name collisions

If you previously used **`Expect`** to avoid clashing with NUnit's `Assert`, prefer **`Ensure`** in v2:

```csharp
// v1
Expect.Throws<ArgumentException>(() => act());

// v2
Ensure.Throws<ArgumentException>(() => act());
// or
((Action)act).Throws<ArgumentException>();
```

The `Expect` type remains as a thin wrapper in v2 but **will be removed in v3.0**; prefer `Ensure` for new code.

## Nullable assertions

```csharp
// v1
maybeUser.VerifyNullable().NotToBeNull();

// v2
maybeUser.VerifyNullable().NotBeNull();
```

Only the assertion method names change; the entry point is still `VerifyNullable()`.

## Compile-time interceptors

Interceptor behaviour is unchanged. Opt-out and `InterceptorsNamespaces` settings are the same as v1—see the [README](README.md#compile-time-boolean-diagnostics-interceptors).

Generated interceptor code now targets **`Ensure.Must(...).BeTrue()`** and **`Ensure.Expression(...)`** instead of legacy `Assert.Verify(...).ToBeTrue()` / `VerifyExpression(...)`.

## Recommended migration steps

1. **Upgrade** the `OmniAssert` package to v2.x.
2. **Build** and review OA001–OA004 warnings.
3. **Run code fixes** (per file or fix all in solution).
4. **Search** for any remaining `.Verify()`, `.ToBe`, or `OmniAssert.Assert` usages your IDE may have missed.
5. **Run tests** and confirm failure messages still read as expected.
6. **Treat warnings as errors** (optional) to prevent reintroducing legacy syntax:

   ```xml
   <PropertyGroup>
     <TreatWarningsAsErrors>true</TreatWarningsAsErrors>
     <WarningsNotAsErrors>OA001;OA002;OA003;OA004</WarningsNotAsErrors>
   </PropertyGroup>
   ```

   Remove `WarningsNotAsErrors` once migration is complete to enforce the new API strictly.

## Need help?

Open an [issue](https://github.com/bolorundurowb/OmniAssert/issues) with a minimal repro if a code fix does not apply cleanly or if you find a v1 pattern with no v2 equivalent.

