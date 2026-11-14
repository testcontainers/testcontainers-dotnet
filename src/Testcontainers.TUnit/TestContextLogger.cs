namespace Testcontainers.TUnit;

/// <summary>
/// Writes log messages to the output of the TUnit context (test, class, assembly, or session) that is current when the message is logged.
/// Messages logged outside of a TUnit context are ignored.
/// </summary>
internal sealed class TestContextLogger : Logger
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    /// <inheritdoc />
    protected override void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        var context = Context.Current;

        if (context == null)
        {
            return;
        }

        var message = GetMessage(state, exception, formatter);
        context.OutputWriter.WriteLine($@"[testcontainers.org {_stopwatch.Elapsed:hh\:mm\:ss\.fff}] {message}");
    }
}