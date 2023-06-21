namespace DotNet.Testcontainers
{
  using System;
  using System.Diagnostics;
  using System.Runtime.InteropServices;
  using JetBrains.Annotations;
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
  [PublicAPI]
  public sealed class ConsoleLogger : ILogger, IDisposable
  {
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private LogLevel _minLogLevel = LogLevel.Information;

    private ConsoleLogger()
    {
      if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && !Console.IsOutputRedirected && !Console.IsErrorRedirected)
      {
        Console.BufferWidth = short.MaxValue - 1;
      }
    }

    /// <summary>
    /// Gets the <see cref="ConsoleLogger" /> instance.
    /// </summary>
    public static ConsoleLogger Instance { get; }
      = new ConsoleLogger();

    /// <summary>
    /// Gets a value indicating whether the debug log level is enabled or not.
    /// </summary>
    public bool DebugLogLevelEnabled
    {
      get
      {
        return LogLevel.Debug.Equals(_minLogLevel);
      }

      set
      {
        _minLogLevel = value ? LogLevel.Debug : LogLevel.Information;
      }
    }

    /// <inheritdoc />
    public void Dispose()
    {
      // The default console logger does not support scopes. We return itself as IDisposable implementation.
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
      if (IsEnabled(logLevel))
      {
        Console.Out.WriteLine("[testcontainers.org {0:hh\\:mm\\:ss\\.ff}] {1}", _stopwatch.Elapsed, formatter.Invoke(state, exception));
      }
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
      return logLevel >= _minLogLevel;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
      return this;
    }
  }
}
