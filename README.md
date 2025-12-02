# SingleFileTestRunner

[![NuGet](https://img.shields.io/nuget/v/Ardalis.SingleFileTestRunner.svg)](https://www.nuget.org/packages/Ardalis.SingleFileTestRunner)
[![NuGet Downloads](https://img.shields.io/nuget/dt/Ardalis.SingleFileTestRunner.svg)](https://www.nuget.org/packages/Ardalis.SingleFileTestRunner)

A lightweight package for running xUnit tests in single-file C# applications. Perfect for quick test scripts, learning scenarios, or standalone test executables.

## Installation

```bash
dotnet add package Ardalis.SingleFileTestRunner
dotnet add package xunit
```

## Quick Start

Create a single `.cs` file with your tests and run them directly:

```csharp
// MyTests.cs
using Xunit;
using Ardalis.SingleFileTestRunner;

// Run all tests in this file
return await TestRunner.RunTestsAsync();

// Your test classes
public class CalculatorTests
{
    [Fact]
    public void Addition_ReturnsCorrectSum()
    {
        Assert.Equal(4, 2 + 2);
    }

    [Theory]
    [InlineData(1, 1, 2)]
    [InlineData(5, 3, 8)]
    [InlineData(-1, 1, 0)]
    public void Addition_WithVariousInputs_ReturnsExpectedResult(int a, int b, int expected)
    {
        Assert.Equal(expected, a + b);
    }
}

public class StringTests
{
    [Fact]
    public void Contains_FindsSubstring()
    {
        Assert.Contains("world", "Hello world!");
    }
}
```

Run with:

```bash
dotnet run MyTests.cs
```

## Features

- **Simple API**: Just call `TestRunner.RunTestsAsync()` to discover and run all xUnit tests
- **Colorized Output**: Clear visual feedback with green for passing, red for failing, and yellow for skipped tests
- **CI/CD Ready**: Returns exit code 0 for success, 1 for failures
- **Single-File Compatible**: Works with `dotnet run` on single `.cs` files and published single-file executables
- **Full xUnit Support**: Supports `[Fact]`, `[Theory]`, and all standard xUnit attributes

## Sample Output

```bash
Discovering and running tests...

  [PASS] CalculatorTests.Addition_ReturnsCorrectSum
  [PASS] CalculatorTests.Addition_WithVariousInputs_ReturnsExpectedResult(a: 1, b: 1, expected: 2)
  [PASS] CalculatorTests.Addition_WithVariousInputs_ReturnsExpectedResult(a: 5, b: 3, expected: 8)
  [PASS] CalculatorTests.Addition_WithVariousInputs_ReturnsExpectedResult(a: -1, b: 1, expected: 0)
  [PASS] StringTests.Contains_FindsSubstring

Test run completed in 0.15s
Total tests: 5
  Passed: 5
```

## Use Cases

- **Learning & Experimentation**: Quickly test C# concepts without setting up a full test project
- **Script-Based Testing**: Create standalone test scripts that can be shared and run easily
- **Single-File Deployments**: Build self-contained test executables for distribution
- **Prototyping**: Rapidly prototype and validate code in a single file
- **Parallel Testing**: Run many test files in completely standalone processes, in parallel

## Requirements

- .NET 10.0 or later
