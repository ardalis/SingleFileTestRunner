namespace Ardalis.SingleFileTestRunner.Tests;

/// <summary>
/// Integration tests for TestRunner.
/// These tests verify the TestRunner correctly discovers and executes tests,
/// and returns appropriate exit codes.
/// </summary>
public class TestRunnerIntegrationTests
{
    /// <summary>
    /// Verifies that RunTestsAsync returns 0 when no tests fail.
    /// Note: When run under the xUnit test host, Environment.ProcessPath points to 
    /// the test host executable rather than the test assembly, so no tests are discovered.
    /// This test verifies the runner handles that scenario gracefully (0 tests = success).
    /// For full integration testing, use a standalone single-file executable.
    /// </summary>
    [Fact]
    public async Task RunTestsAsync_WhenNoTestsDiscovered_ReturnsZero()
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
            Assert.Equal(0, result);
            
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
