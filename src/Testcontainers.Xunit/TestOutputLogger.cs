namespace Testcontainers.Xunit;

internal sealed class TestOutputLogger(ITestOutputHelper testOutputHelper) : ILogger, IDisposable
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        testOutputHelper.WriteLine($@"[testcontainers.org {_stopwatch.Elapsed:hh\:mm\:ss\.fff}] {formatter(state, exception)}");
        if (exception != null)
        {
            testOutputHelper.WriteLine(exception.ToString());
        }
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => this;

    public void Dispose()
    {
    }
}