namespace Testcontainers.Xunit;

internal sealed class MessageSinkLogger(IMessageSink messageSink) : ILogger, IDisposable
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = exception == null ? formatter(state, null) : $"{formatter(state, exception)}{Environment.NewLine}{exception}";
        messageSink.OnMessage(new DiagnosticMessage($"[testcontainers.org] {message}"));
    }

    public bool IsEnabled(LogLevel logLevel) => true;

    public IDisposable BeginScope<TState>(TState state) => this;

    public void Dispose()
    {
    }

    /// <returns>
    /// The hash code of the underlying message sink, because <see cref="DotNet.Testcontainers.Clients.DockerApiClient.LogContainerRuntimeInfoAsync"/>
    /// logs the runtime information once per Docker Engine API client and logger.
    /// </returns>
    public override int GetHashCode() => messageSink.GetHashCode();
}