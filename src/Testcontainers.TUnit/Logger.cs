namespace Testcontainers.TUnit;

/// <summary>
/// Base class for loggers that forward Testcontainers' log messages to a test framework output.
/// </summary>
internal abstract class Logger : ILogger
{
    /// <summary>
    /// Formats the log message and appends the exception, if any.
    /// </summary>
    /// <param name="state">The entry to be written.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="string" /> message of the state and exception.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    /// <returns>The formatted log message.</returns>
    protected static string GetMessage<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        return exception == null ? formatter(state, null) : $"{formatter(state, exception)}{Environment.NewLine}{exception}";
    }

    /// <summary>
    /// Writes the log entry to the test framework output.
    /// </summary>
    /// <param name="state">The entry to be written.</param>
    /// <param name="exception">The exception related to this entry.</param>
    /// <param name="formatter">Function to create a <see cref="string" /> message of the state and exception.</param>
    /// <typeparam name="TState">The type of the object to be written.</typeparam>
    protected abstract void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter);

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        Log(state, exception, formatter);
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state) => new NullScope();
}