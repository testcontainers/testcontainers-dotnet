namespace Testcontainers.Xunit;

internal sealed class XunitLoggerProvider : ILoggerProvider
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    private readonly ITestOutputHelper _testOutputHelper;

    public XunitLoggerProvider(IMessageSink messageSink)
    {
        _testOutputHelper = new MessageSinkTestOutputHelper(messageSink);
    }

    public XunitLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new XunitLogger(_stopwatch, _testOutputHelper, categoryName);
    }

    private sealed class MessageSinkTestOutputHelper : ITestOutputHelper
    {
        private readonly IMessageSink _messageSink;

#if XUNIT_V3
        public string Output => throw new NotImplementedException();
#endif

        public MessageSinkTestOutputHelper(IMessageSink messageSink)
        {
            _messageSink = messageSink;
        }

        public void WriteLine(string message)
        {
            _messageSink.OnMessage(new DiagnosticMessage(message));
        }

        public void WriteLine(string format, params object[] args)
        {
            _messageSink.OnMessage(new DiagnosticMessage(format, args));
        }
    }

    private sealed class XunitLogger : ILogger
    {
        private readonly Stopwatch _stopwatch;

        private readonly ITestOutputHelper _testOutputHelper;

        private readonly string _categoryName;

        public XunitLogger(Stopwatch stopwatch, ITestOutputHelper testOutputHelper, string categoryName)
        {
            _stopwatch = stopwatch;
            _testOutputHelper = testOutputHelper;
            _categoryName = categoryName;
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            return Disposable.Instance;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return logLevel != LogLevel.None;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var message = exception == null ? formatter.Invoke(state, null) : $"{formatter.Invoke(state, exception)}\n{exception}";
            _testOutputHelper.WriteLine("[{0} {1:hh\\:mm\\:ss\\.ff}] {2}", _categoryName, _stopwatch.Elapsed, message);
        }

        private sealed class Disposable : IDisposable
        {
            private Disposable()
            {
            }

            public static IDisposable Instance { get; } = new Disposable();

            public void Dispose()
            {
            }
        }
    }
}