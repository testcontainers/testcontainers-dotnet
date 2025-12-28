namespace Testcontainers.Xunit;

internal sealed class TestOutputLogger(ITestOutputHelper testOutputHelper) : Logger
{
    private readonly Stopwatch _stopwatch = Stopwatch.StartNew();

    protected override void Log<TState>(TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (testOutputHelper == null)
        {
            return;
        }

        var message = GetMessage(state, exception, formatter);
        testOutputHelper.WriteLine($@"[testcontainers.org {_stopwatch.Elapsed:hh\:mm\:ss\.fff}] {message}");
    }
}