namespace Testcontainers.Tests;

public sealed class ContainerNotRunningExceptionTest
{
    public static TheoryData<ContainerInfoSerializable, string> ContainerTestData { get; }
        = new TheoryData<ContainerInfoSerializable, string>
        {
            {
                new ContainerInfoSerializable("container-id", "Stdout\nStdout", "Stderr\nStderr", 1),
                "Container container-id exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            },
            {
                new ContainerInfoSerializable("container-id", "Stdout\nStdout", string.Empty, 1),
                "Container container-id exited with code 1." + Environment.NewLine +
                "  Stdout: " + Environment.NewLine +
                "    Stdout" + Environment.NewLine +
                "    Stdout"
            },
            {
                new ContainerInfoSerializable("container-id", string.Empty, "Stderr\nStderr", 1),
                "Container container-id exited with code 1." + Environment.NewLine +
                "  Stderr: " + Environment.NewLine +
                "    Stderr" + Environment.NewLine +
                "    Stderr"
            },
            {
                new ContainerInfoSerializable("container-id", string.Empty, string.Empty, 1),
                "Container container-id exited with code 1."
            },
        };

    [Theory]
    [MemberData(nameof(ContainerTestData))]
    public void ContainerNotRunningExceptionCreatesExpectedMessage(ContainerInfoSerializable serializable, string message)
    {
        // Given
        var (id, stdout, stderr, exitCode) = serializable.ToTuple();

        // When
        var exception = new ContainerNotRunningException(id, stdout, stderr, exitCode, null);

        // Then
        Assert.Equal(message, exception.Message);
    }

    [PublicAPI]
    public sealed class ContainerInfoSerializable : IXunitSerializable
    {
        private string _id;

        private string _stdout;

        private string _stderr;

        private long _exitCode;

        public ContainerInfoSerializable()
        {
        }

        public ContainerInfoSerializable(string id, string stdout, string stderr, long exitCode)
        {
            _id = id;
            _stdout = stdout;
            _stderr = stderr;
            _exitCode = exitCode;
        }

        public (string Id, string Stdout, string Stderr, long ExitCode) ToTuple()
        {
            return (_id, _stdout, _stderr, _exitCode);
        }

        public void Deserialize(IXunitSerializationInfo info)
        {
            _id = info.GetValue<string>(nameof(_id));
            _stdout = info.GetValue<string>(nameof(_stdout));
            _stderr = info.GetValue<string>(nameof(_stderr));
            _exitCode = info.GetValue<long>(nameof(_exitCode));
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue(nameof(_id), _id);
            info.AddValue(nameof(_stdout), _stdout);
            info.AddValue(nameof(_stderr), _stderr);
            info.AddValue(nameof(_exitCode), _exitCode);
        }
    }
}