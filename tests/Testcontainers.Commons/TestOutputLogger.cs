using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace DotNet.Testcontainers.Commons;

public class TestOutputLogger : ILogger
{
    private readonly string _categoryName;
    private readonly LogLevel _logLevel;
    private readonly ITestOutputHelper _testOutputHelper;
        
    public TestOutputLogger(string categoryName, ITestOutputHelper testOutputHelper, LogLevel logLevel = LogLevel.Information)
    {
        _categoryName = categoryName;
        _testOutputHelper = testOutputHelper;
        _logLevel = logLevel;
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        if (formatter == null)
            throw new ArgumentNullException(nameof(formatter));

        var message = formatter(state, exception);
        if (string.IsNullOrEmpty(message))
        {
            return;
        }

        _testOutputHelper.WriteLine($"{logLevel}: {_categoryName}: {message}");
    }

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
    {
        return logLevel >= _logLevel;
    }

    /// <inheritdoc />
    public IDisposable BeginScope<TState>(TState state)
    {
        return null!;
    }
    
}