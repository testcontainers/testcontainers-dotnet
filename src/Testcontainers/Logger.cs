namespace DotNet.Testcontainers
{
  using System;
  using System.Diagnostics;
  using System.Globalization;
  using System.IO;
  using System.Runtime.InteropServices;
  using Microsoft.Extensions.Logging;

  /// <summary>
  /// An <see cref="ILogger" /> implementation that forwards messages to the console. Not every test framework or environment supports this approach. Developers may still need to configure their own logging implementation.
  /// If VSTest.Console.exe loads the test adapter in a deterministic order, we can write our own test adapter and intercept the IMessageLogger instance: https://github.com/microsoft/vstest/issues/4125#issuecomment-1320880502.
  /// To debug the test host and runner set the environment variables VSTEST_HOST_DEBUG and VSTEST_RUNNER_DEBUG to 1. To enable VSTest logging set VSTEST_DIAG to 1 and VSTEST_DIAG_VERBOSITY to verbose.
  /// The following example contains the ITestExecutor implementations. It is important that the assembly ends with TestAdapter.dll.
  /// </summary>
  /// <example>
  /// <code>
  ///   [FileExtension(DllFileExtension)]
  ///   [FileExtension(ExeFileExtension)]
  ///   [DefaultExecutorUri(ExecutorUri)]
  ///   [ExtensionUri(ExecutorUri)]
  ///   [Category(Category)]
  ///   internal sealed class UssDiscovery : ITestDiscoverer, ITestExecutor
  ///   {
  ///     private const string DllFileExtension = &quot;.dll&quot;;
  ///
  ///     private const string ExeFileExtension = &quot;.exe&quot;;
  ///
  ///     private const string ExecutorUri = &quot;executor://testcontainers.org/v1&quot;;
  ///
  ///     private const string Category = &quot;managed&quot;;
  ///
  ///     public void DiscoverTests(IEnumerable&lt;string&gt; sources, IDiscoveryContext discoveryContext, IMessageLogger logger, ITestCaseDiscoverySink discoverySink)
  ///     {
  ///     }
  ///
  ///     public void RunTests(IEnumerable&lt;TestCase&gt; tests, IRunContext runContext, IFrameworkHandle frameworkHandle)
  ///     {
  ///       SetLogger(frameworkHandle);
  ///     }
  ///
  ///     public void RunTests(IEnumerable&lt;string&gt; sources, IRunContext runContext, IFrameworkHandle frameworkHandle)
  ///     {
  ///       SetLogger(frameworkHandle);
  ///     }
  ///
  ///     public void Cancel()
  ///     {
  ///     }
  ///
  ///     private static void SetLogger(IMessageLogger logger)
  ///     {
  ///       // Set the TestcontainersSettings.Logger. Use a semaphore to block the test execution until the logger is set.
  ///     }
  ///   }
  /// </code>
  /// </example>
  internal sealed class Logger : ILogger, IDisposable
  {
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public Logger()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Console.IsOutputRedirected && !Console.IsErrorRedirected)
      {
        Console.BufferWidth = short.MaxValue - 1;
      }
    }

    public void Dispose()
    {
      // The default logger does not support scopes. We return itself as IDisposable implementation.
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      TextWriter console;

      switch (logLevel)
      {
        case LogLevel.Information:
          console = Console.Out;
          break;
        case LogLevel.Warning:
          console = Console.Out;
          break;
        case LogLevel.Error:
          console = Console.Error;
          break;
        case LogLevel.Critical:
          console = Console.Error;
          break;
        default:
          return;
      }

      var message = string.Format(CultureInfo.CurrentCulture, "[testcontainers.org {0:hh\\:mm\\:ss\\.ff}] {1}", this.stopwatch.Elapsed, formatter.Invoke(state, exception));
      console.WriteLine(message);
    }

    public bool IsEnabled(LogLevel logLevel)
    {
      return true;
    }

    public IDisposable BeginScope<TState>(TState state)
    {
      return this;
    }
  }
}
