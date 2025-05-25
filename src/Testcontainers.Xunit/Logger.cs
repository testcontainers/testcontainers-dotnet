namespace Testcontainers.Xunit;

internal abstract class Logger : ILogger
{
    protected static string GetMessage<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        return exception == null ? formatter(state, null) : $"{formatter(state, exception)}{Environment.NewLine}{exception}";
    }

    protected abstract void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter);

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Log(state, exception, formatter);
    }

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public IDisposable BeginScope<TState>(TState state) => new NullScope();
}