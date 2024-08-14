namespace Testcontainers.Xunit;

internal sealed class TestContextLogger : ILogger
{
    private TestContextLogger()
    {
    }

    public static TestContextLogger Instance { get; } = new TestContextLogger();

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = exception == null ? formatter(state, null) : $"{formatter(state, exception)}{Environment.NewLine}{exception}";
        TestContext.Current.SendDiagnosticMessage($"[testcontainers.org] {message}");
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public IDisposable BeginScope<TState>(TState state) => new NullScope();
}
