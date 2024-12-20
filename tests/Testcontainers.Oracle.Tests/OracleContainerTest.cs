namespace Testcontainers.Oracle;

public sealed class OracleContainerTest(OracleContainerTest.OracleFixture oracleFixture) : IClassFixture<OracleContainerTest.OracleFixture>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = oracleFixture.CreateConnection();

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1 FROM DUAL;";

        // When
        var execResult = await oracleFixture.Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }

    [UsedImplicitly]
    public class OracleFixture(IMessageSink messageSink) : DbContainerFixture<OracleBuilder, OracleContainer>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => OracleClientFactory.Instance;
    }
}