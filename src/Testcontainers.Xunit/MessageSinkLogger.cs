namespace Testcontainers.Xunit;

internal sealed class MessageSinkLogger(IMessageSink messageSink) : Logger
{
    protected override void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = GetMessage(state, exception, formatter);
        messageSink.OnMessage(new DiagnosticMessage($"[testcontainers.org] {message}"));
    }

    /// <returns>
    /// The hash code of the underlying message sink, because <see cref="DotNet.Testcontainers.Clients.DockerApiClient.LogContainerRuntimeInfoAsync"/>
    /// logs the runtime information once per Docker Engine API client and logger.
    /// </returns>
    public override int GetHashCode() => messageSink.GetHashCode();
}