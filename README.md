# OmniAssert

[![Build, Test & Coverage](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml/badge.svg)](https://github.com/bolorundurowb/OmniAssert/actions/workflows/build-and-test.yml) [![codecov](https://codecov.io/gh/bolorundurowb/OmniAssert/graph/badge.svg?token=J9ssbN9xYA)](https://codecov.io/gh/bolorundurowb/OmniAssert) [![License](https://img.shields.io/badge/license-GPLv3-orange.svg)](./LICENSE)

OmniAssert is a lightweight, fluent assertion library for .NET designed for clarity and rich failure diagnostics. It combines a natural-language API with compile-time capture of operand values for boolean conditions.

## Key Features

- **Fluent Assertions**: Natural `Verify(subject).To...` syntax for common types.
- **Rich Boolean Failures**: Optional MSBuild-based rewriting to capture operand values in `Verify(a == b)`-style checks.
- **Soft Assertions**: `AssertionScope` allows collecting multiple failures before throwing.
- **Object Equivalence**: Deep structural comparison of objects with detailed diffs.
- **Diagnostic Output**: Failures include source expressions and colorized terminal output.

---

## Consumer Guide

### Getting Started

1.  **Reference the library**: Add a reference to `OmniAssert.Core` in your test project.
2.  **Import the API**:
    ```csharp
    using static OmniAssert.Assert;
    ```

### Basic Assertions

#### Numeric
Supports all types implementing `INumber<T>` (int, long, double, decimal, etc.).
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
Verify(actualList).ToBeEquivalentTo(expectedList); // Unordered comparison
```

#### Nullability
```csharp
VerifyNullable(someObject).NotToBeNull();
Verify(someString).ToBeNullOrWhiteSpace();
```

#### Exceptions
```csharp
Throws<ArgumentException>(() => someObject.DoWork());
await ThrowsAsync<InvalidOperationException>(async () => await someTask);

// Verify exception properties
Throws<Exception>(() => action()).WithMessage("Error").WithInnerException<SocketException>();
```

#### Types & Dates
```csharp
VerifyType(instance).ToBeOfType<MyClass>();
Verify(dateTime).ToBeAfter(DateTime.Now.AddDays(-1));
```

### Advanced Usage

#### Assertion Scopes (Soft Assertions)
Collect multiple failures and report them all at once when the scope is disposed.
```csharp
using (new AssertionScope())
{
    Verify(user.Name).ToBe("John");
    Verify(user.Age).ToBe(30);
}
```

#### Object Equivalence
Perform a deep property-by-property comparison.
```csharp
VerifyEquivalent(expectedDto, actualDto);
```

### Enabling Boolean Rewriting
To see operand values in `Verify(x == y)` failures, add the following to your `.csproj`:

1.  Reference the build task:
    ```xml
    <ProjectReference Include="path\to\OmniAssert.Build.csproj" ReferenceOutputAssembly="false" />
    ```
2.  Import the targets:
    ```xml
    <Import Project="path\to\OmniAssert.Build\build\OmniAssert.Build.targets" />
    ```
3.  Enable the feature:
    ```xml
    <PropertyGroup>
      <OmniAssertRewrite>true</OmniAssertRewrite>
    </PropertyGroup>
    ```

---

## Contributor Guide

### Repository Layout

| Path | Role |
|------|------|
| `src/OmniAssert.Core` | Runtime API and assertion implementations. |
| `src/OmniAssert.Build` | MSBuild task for boolean expression rewriting. |
| `src/OmniAssert.Generator` | Roslyn incremental generator (future use). |
| `tests/OmniAssert.Tests` | Comprehensive test suite. |

### Build and Test

Projects target **.NET 10**. Use the following commands for development:

```bash
# Build
dotnet build src/OmniAssert.Core/OmniAssert.Core.csproj

# Run Tests
dotnet test tests/OmniAssert.Tests/OmniAssert.Tests.csproj

# Run Tests with Coverage
dotnet test tests/OmniAssert.Tests/OmniAssert.Tests.csproj /p:CollectCoverage=true
```

### Test Naming Convention
Tests should follow the pattern: `MethodUnderTest_Scenario_ExpectedBehavior` (e.g., `Dispose_WhenNoFailures_ShouldNotThrow`).

## License
This project is licensed under the GNU General Public License v3.0. See [LICENSE](LICENSE).
