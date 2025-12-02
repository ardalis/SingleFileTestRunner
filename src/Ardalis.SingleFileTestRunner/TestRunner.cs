using Xunit.Runners;

namespace Ardalis.SingleFileTestRunner;

/// <summary>
/// A simple test runner for single-file xUnit tests.
/// </summary>
public static class TestRunner
{
    /// <summary>
    /// Discovers and runs all xUnit tests in the current assembly.
    /// </summary>
    /// <returns>0 if all tests pass, 1 if any tests fail.</returns>
    public static async Task<int> RunTestsAsync()
    {
        // Use Environment.ProcessPath for single-file apps (Assembly.Location returns empty string)
        var assemblyPath = Environment.ProcessPath!;

        Console.WriteLine("Discovering and running tests...\n");

        var runner = AssemblyRunner.WithoutAppDomain(assemblyPath);

        int passed = 0, failed = 0, skipped = 0;
        var startTime = DateTime.Now;
        var consoleLock = new object();

        runner.OnTestFailed = info =>
        {
            lock (consoleLock)
            {
                failed++;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  [FAIL] ");
                Console.ResetColor();
                Console.WriteLine(info.TestDisplayName);
                Console.WriteLine($"         {info.ExceptionMessage}");
                if (!string.IsNullOrEmpty(info.ExceptionStackTrace))
                {
                    foreach (var line in info.ExceptionStackTrace.Split('\n'))
                    {
                        Console.WriteLine($"         {line}");
                    }
                }
            }
        };

        runner.OnTestPassed = info =>
        {
            lock (consoleLock)
            {
                passed++;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("  [PASS] ");
                Console.ResetColor();
                Console.WriteLine(info.TestDisplayName);
            }
        };

        runner.OnTestSkipped = info =>
        {
            lock (consoleLock)
            {
                skipped++;
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("  [SKIP] ");
                Console.ResetColor();
                Console.WriteLine($"{info.TestDisplayName} - {info.SkipReason}");
            }
        };

        var tcs = new TaskCompletionSource<bool>();
        runner.OnExecutionComplete = info => tcs.SetResult(true);

        runner.Start();
        await tcs.Task;
        await Task.Delay(100); // Allow runner to transition to idle state before disposal
        runner.Dispose();

        var duration = DateTime.Now - startTime;

        // Display results
        Console.WriteLine($"\nTest run completed in {duration.TotalSeconds:F2}s");
        Console.WriteLine($"Total tests: {passed + failed + skipped}");

        if (passed > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"  Passed: {passed}");
            Console.ResetColor();
            Console.WriteLine();
        }

        if (failed > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"  Failed: {failed}");
            Console.ResetColor();
            Console.WriteLine();
        }

        if (skipped > 0)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"  Skipped: {skipped}");
            Console.ResetColor();
            Console.WriteLine();
        }

        // Exit with appropriate code for CI/CD integration
        return failed > 0 ? 1 : 0;
    }
}
