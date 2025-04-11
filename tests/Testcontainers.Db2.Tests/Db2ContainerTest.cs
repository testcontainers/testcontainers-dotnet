namespace Testcontainers.Db2;

public sealed class Db2ContainerTest(Db2ContainerTest.Db2Fixture fixture) : IClassFixture<Db2ContainerTest.Db2Fixture>
{
    // # --8<-- [start:UseDb2Container]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = fixture.CreateConnection();

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
        const string scriptContent = "SELECT 1 FROM SYSIBM.SYSDUMMY1;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseDb2Container]

    [UsedImplicitly]
    public class Db2Fixture(IMessageSink messageSink) : DbContainerFixture<Db2Builder, Db2Container>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory => DB2Factory.Instance;

        protected override Db2Builder Configure(Db2Builder builder) => builder.WithAcceptLicenseAgreement(true);
    }
}