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

        testOutputHelper.WriteLine($@"[testcontainers.org {_stopwatch.Elapsed:hh\:mm\:ss\.fff}] {formatter(state, exception)}");

        if (exception != null)
        {
            testOutputHelper.WriteLine(exception.ToString());
        }
    }
}