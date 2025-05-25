namespace Testcontainers.Xunit;

internal sealed class MessageSinkLogger(IMessageSink messageSink) : Logger
{
    private readonly IMessageSink _messageSink = messageSink;

    protected override void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (_messageSink == null)
        {
            return;
        }

        var message = GetMessage(state, exception, formatter);
        _messageSink.OnMessage(new DiagnosticMessage($"[testcontainers.org] {message}"));
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj is MessageSinkLogger other)
        {
            return Equals(_messageSink, other._messageSink);
        }

        return false;
    }

    /// <returns>
    /// The hash code of the underlying message sink, because <see cref="DotNet.Testcontainers.Clients.DockerApiClient.LogContainerRuntimeInfoAsync" />
    /// logs the runtime information once per Docker Engine API client and logger.
    /// </returns>
    public override int GetHashCode() => _messageSink?.GetHashCode() ?? 0;
}