namespace Testcontainers.LocalStack;

public sealed class LocalStackBuilderTest
{
    [Fact]
    public void LocalStack415WithoutAuthTokenThrowsArgumentException()
    {
        const string message = "The image 'localstack/localstack:4.15.0' requires the LOCALSTACK_AUTH_TOKEN environment variable for LocalStack 4.15 and onwards.";
        ExpectArgEx(message, () => new LocalStackBuilder("localstack/localstack:4.15.0").Build());
    }

    [Fact]
    public void LocalStack500WithoutAuthTokenThrowsArgumentException()
    {
        const string message = "The image 'localstack/localstack:5.0.0' requires the LOCALSTACK_AUTH_TOKEN environment variable for LocalStack 4.15 and onwards.";
        ExpectArgEx(message, () => new LocalStackBuilder("localstack/localstack:5.0.0").Build());
    }

    [Fact]
    public void LocalStack414WithoutAuthTokenDoesNotThrowArgumentException()
    {
        var exception = Xunit.Record.Exception(() => new LocalStackBuilder("localstack/localstack:4.14.0").Build());
        Assert.Null(exception);
    }

    [Fact]
    public void LocalStack415WithAuthTokenDoesNotThrowArgumentException()
    {
        var exception = Xunit.Record.Exception(() => new LocalStackBuilder("localstack/localstack:4.15.0").WithEnvironment("LOCALSTACK_AUTH_TOKEN", "<auth-token>").Build());
        Assert.Null(exception);
    }

    private static void ExpectArgEx(string expectedStartString, Action testCode)
    {
        var exception = Assert.Throws<ArgumentException>(testCode);
        Assert.StartsWith(expectedStartString, exception.Message);
    }
}