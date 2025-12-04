using Xunit.Runner.Common;
using Xunit.Runner.InProc.SystemConsole;
using Xunit.Sdk;
using System.Reflection;

namespace Ardalis.SingleFileTestRunner;

/// <summary>
/// A simple test runner for single-file xUnit v3 tests.
/// </summary>
public static class TestRunner
{
    // Track if we're already running to prevent recursive execution
    private static int _isRunning = 0;
    
    /// <summary>
    /// Discovers and runs all xUnit tests in the current assembly.
    /// </summary>
    /// <returns>0 if all tests pass, 1 if any tests fail.</returns>
    public static async Task<int> RunTestsAsync()
    {
        // Prevent recursive execution when tests call RunTestsAsync
        if (Interlocked.CompareExchange(ref _isRunning, 1, 0) != 0)
        {
            return 0; // Already running, return success
        }

        try
        {
            return await RunTestsAsyncCore();
        }
        finally
        {
            Interlocked.Exchange(ref _isRunning, 0);
        }
    }

    private static async Task<int> RunTestsAsyncCore()
    {
        // Use Environment.ProcessPath for single-file apps (Assembly.Location returns empty string).
        // For test scenarios under test hosts, ProcessPath points to the test host, not the test DLL.
        // Use Assembly.Location as fallback only if ProcessPath is not a DLL.
        var processPath = Environment.ProcessPath!;
        var entryAssembly = Assembly.GetEntryAssembly();
        
        string assemblyPath;
        if (processPath.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
        {
            assemblyPath = processPath;
        }
        else if (entryAssembly != null && !string.IsNullOrEmpty(entryAssembly.Location))
        {
            assemblyPath = entryAssembly.Location;
        }
        else
        {
            assemblyPath = processPath;
        }

        Console.WriteLine("Discovering and running tests...\n");

        int passed = 0, failed = 0, skipped = 0;
        var startTime = DateTime.Now;
        var consoleLock = new object();
        
        // Track test display names using test unique IDs
        var testNameMap = new Dictionary<string, string>();

        var sink = new TestMessageSink();
        
        // Track test names as they start
        sink.Execution.TestStartingEvent += args =>
        {
            testNameMap[args.Message.TestUniqueID] = args.Message.TestDisplayName;
        };

        sink.Execution.TestFailedEvent += args =>
        {
            lock (consoleLock)
            {
                failed++;
                var testName = testNameMap.TryGetValue(args.Message.TestUniqueID, out var name) ? name : "Unknown test";
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("  [FAIL] ");
                Console.ResetColor();
                Console.WriteLine(testName);
                if (args.Message.Messages != null && args.Message.Messages.Length > 0 && args.Message.Messages[0] != null)
                {
                    Console.WriteLine($"         {args.Message.Messages[0]}");
                }
                if (args.Message.StackTraces != null && args.Message.StackTraces.Length > 0 && args.Message.StackTraces[0] != null && !string.IsNullOrEmpty(args.Message.StackTraces[0]))
                {
                    foreach (var line in args.Message.StackTraces[0]!.Split('\n'))
                    {
                        Console.WriteLine($"         {line}");
                    }
                }
            }
        };

        sink.Execution.TestPassedEvent += args =>
        {
            lock (consoleLock)
            {
                passed++;
                var testName = testNameMap.TryGetValue(args.Message.TestUniqueID, out var name) ? name : "Unknown test";
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("  [PASS] ");
                Console.ResetColor();
                Console.WriteLine(testName);
            }
        };

        sink.Execution.TestSkippedEvent += args =>
        {
            lock (consoleLock)
            {
                skipped++;
                var testName = testNameMap.TryGetValue(args.Message.TestUniqueID, out var name) ? name : "Unknown test";
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write("  [SKIP] ");
                Console.ResetColor();
                Console.WriteLine($"{testName} - {args.Message.Reason}");
            }
        };

        // Run tests using ConsoleRunnerInProcess
        var cts = new CancellationTokenSource();
        var assembly = Assembly.LoadFrom(assemblyPath);
        
        // Create XunitProjectAssembly for the test assembly
        var project = new XunitProject();
        var targetFramework = assembly.GetCustomAttribute<System.Runtime.Versioning.TargetFrameworkAttribute>()?.FrameworkName 
            ?? $".NETCoreApp,Version=v{Environment.Version.Major}.{Environment.Version.Minor}";
        var metadata = new AssemblyMetadata(3, targetFramework); // xUnit v3
        var projectAssembly = new XunitProjectAssembly(project, assemblyPath, metadata)
        {
            Assembly = assembly,
            ConfigFileName = null,
        };

        await ConsoleRunnerInProcess.Run(sink, sink.Diagnostics, projectAssembly, cts);

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
