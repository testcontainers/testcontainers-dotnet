namespace Testcontainers.Tests;

public sealed class ExecFailedExceptionTest
{
    public static readonly List<TheoryDataRow<ExecResult, string>> ExecResultTestData
        = new List<TheoryDataRow<ExecResult, string>>
        {
            new TheoryDataRow<ExecResult, string>
            (
                new ExecResult("Stdout\nStdout", "Stderr\nStderr", 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            ),
            new TheoryDataRow<ExecResult, string>
            (
                new ExecResult("Stdout\nStdout", string.Empty, 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout"
            ),
            new TheoryDataRow<ExecResult, string>
            (
                new ExecResult(string.Empty, "Stderr\nStderr", 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            ),
            new TheoryDataRow<ExecResult, string>
            (
                new ExecResult(string.Empty, string.Empty, 1),
                "Process exited with code 1."
            ),
        };

    [Theory]
    [MemberData(nameof(ExecResultTestData))]
    public void ExecFailedExceptionCreatesExpectedMessage(ExecResult execResult, string message)
    {
        var exception = new ExecFailedException(execResult);
        Assert.Equal(execResult, exception.ExecResult);
        Assert.Equal(message, exception.Message);
    }
}