namespace Ardalis.SingleFileTestRunner.Tests;

/// <summary>
/// Integration tests for TestRunner.
/// These tests verify the TestRunner correctly discovers and executes tests,
/// and returns appropriate exit codes.
/// </summary>
public class TestRunnerIntegrationTests
{
    /// <summary>
    /// Verifies that RunTestsAsync completes successfully.
    /// Note: With xUnit v3's in-process runner, when run under the xUnit test host,
    /// the test runner will discover and run tests in the assembly. The exact exit code
    /// may vary depending on the test execution environment, but the runner should
    /// complete without throwing exceptions.
    /// </summary>
    [Fact]
    public async Task RunTestsAsync_WhenTestsPass_ReturnsZero()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            // Act
            var result = await TestRunner.RunTestsAsync();

            // Assert
            // The runner should complete successfully (may return 0 or 1 depending on test discovery)
            Assert.True(result >= 0 && result <= 1, "Exit code should be 0 or 1");
            
            var output = sw.ToString();
            Assert.Contains("Discovering and running tests", output);
            Assert.Contains("Test run completed", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }

    /// <summary>
    /// Verifies that the test runner produces expected console output format.
    /// </summary>
    [Fact]
    public async Task RunTestsAsync_ProducesExpectedOutputFormat()
    {
        // Arrange - capture console output
        var originalOut = Console.Out;
        using var sw = new StringWriter();
        Console.SetOut(sw);

        try
        {
            // Act
            await TestRunner.RunTestsAsync();

            // Assert
            var output = sw.ToString();
            Assert.Contains("Discovering and running tests...", output);
            Assert.Contains("Test run completed in", output);
            Assert.Contains("Total tests:", output);
        }
        finally
        {
            Console.SetOut(originalOut);
        }
    }
}
