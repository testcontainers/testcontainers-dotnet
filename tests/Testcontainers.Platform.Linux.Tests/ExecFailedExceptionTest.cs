namespace Testcontainers.Tests;

public sealed class ExecFailedExceptionTest
{
    public static TheoryData<ExecResultSerializable, string> ExecResultTestData { get; }
        = new TheoryData<ExecResultSerializable, string>
        {
            {
                new ExecResultSerializable("Stdout\nStdout", "Stderr\nStderr", 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            },
            {
                new ExecResultSerializable("Stdout\nStdout", string.Empty, 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout"
            },
            {
                new ExecResultSerializable(string.Empty, "Stderr\nStderr", 1),
                "Process exited with code 1." + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            },
            {
                new ExecResultSerializable(string.Empty, string.Empty, 1),
                "Process exited with code 1."
            },
        };

    [Theory]
    [MemberData(nameof(ExecResultTestData))]
    public void ExecFailedExceptionCreatesExpectedMessage(ExecResultSerializable serializable, string message)
    {
        // Given
        var execResult = serializable.ToExecResult();

        // When
        var exception = new ExecFailedException(execResult);

        // Then
        Assert.Equal(execResult, exception.ExecResult);
        Assert.Equal(message, exception.Message);
    }

    [PublicAPI]
    public sealed class ExecResultSerializable : IXunitSerializable
    {
        private string _stdout;

        private string _stderr;

        private int _exitCode;

        public ExecResultSerializable()
        {
        }

        public ExecResultSerializable(string stdout, string stderr, int exitCode)
        {
            _stdout = stdout;
            _stderr = stderr;
            _exitCode = exitCode;
        }

        public ExecResult ToExecResult()
        {
            return new ExecResult(_stdout, _stderr, _exitCode);
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            _stdout = info.GetValue<string>(nameof(_stdout));
            _stderr = info.GetValue<string>(nameof(_stderr));
            _exitCode = info.GetValue<int>(nameof(_exitCode));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(_stdout), _stdout);
            info.AddValue(nameof(_stderr), _stderr);
            info.AddValue(nameof(_exitCode), _exitCode);
        }
    }
}