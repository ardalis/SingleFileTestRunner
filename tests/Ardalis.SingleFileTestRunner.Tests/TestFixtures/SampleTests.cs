using Xunit;

namespace Ardalis.SingleFileTestRunner.Tests.TestFixtures;

// ============================================================================
// Sample Test Classes for Integration Testing
// These tests are used to verify the TestRunner behaves correctly
// ============================================================================

public class IntAdditionOperator
{
    [Fact]
    public void ReturnsCorrectSumGivenTwoIntegers()
    {
        Assert.Equal(4, 2 + 2);
    }
}

public class StringContains
{
    [Fact]
    public void ReturnsSubstringWhenPresent()
    {
        Assert.Contains("world", "Hello world!");
    }
}

public class IntModulo2GivenOddNumbers
{
    [Theory]
    [InlineData(3)]
    [InlineData(5)]
    [InlineData(7)]
    public void ReturnsTrue(int value)
    {
        Assert.True(value % 2 == 1);
    }
}

public class IntMultiplicationOperator
{
    [Fact]
    public void ReturnsZeroWhenMultipliedByZero()
    {
        Assert.Equal(0, 5 * 0);
        Assert.Equal(0, 0 * 5);
    }

    [Fact]
    public void ReturnsSameNumberWhenMultipliedByOne()
    {
        Assert.Equal(7, 7 * 1);
        Assert.Equal(7, 1 * 7);
    }

    [Fact]
    public void ReturnsCorrectProductGivenTwoPositiveIntegers()
    {
        Assert.Equal(6, 2 * 3);
        Assert.Equal(20, 4 * 5);
        Assert.Equal(100, 10 * 10);
    }
}

public class SkippedTestExample
{
    [Fact(Skip = "Demonstrating skipped test behavior")]
    public void SkippedTest()
    {
        Assert.Fail("This should not run");
    }
}
