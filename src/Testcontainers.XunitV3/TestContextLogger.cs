namespace Testcontainers.Xunit;

internal sealed class TestContextLogger : Logger
{
    private TestContextLogger()
    {
    }

    public static TestContextLogger Instance { get; } = new TestContextLogger();

    protected override void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var message = GetMessage(state, exception, formatter);
        TestContext.Current.SendDiagnosticMessage($"[testcontainers.org] {message}");
    }
}
