namespace Testcontainers.Db2;

public abstract class Db2ContainerTest(Db2ContainerTest.Db2DefaultFixture fixture)
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
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseDb2Container]

    public class Db2DefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<Db2Builder, Db2Container>(messageSink)
    {
        public override DbProviderFactory DbProviderFactory
            => DB2Factory.Instance;

        protected override Db2Builder Configure(Db2Builder builder)
            => builder.WithImage(TestSession.GetImageFromDockerfile()).WithAcceptLicenseAgreement(true);
    }

    [UsedImplicitly]
    public class Db2WaitForDatabaseFixture(IMessageSink messageSink)
        : Db2DefaultFixture(messageSink)
    {
        protected override Db2Builder Configure(Db2Builder builder)
            => base.Configure(builder).WithImage(TestSession.GetImageFromDockerfile()).WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public sealed class Db2DefaultConfiguration(Db2DefaultFixture fixture)
        : Db2ContainerTest(fixture), IClassFixture<Db2DefaultFixture>;

    [UsedImplicitly]
    public sealed class Db2WaitForDatabaseConfiguration(Db2WaitForDatabaseFixture fixture)
        : Db2ContainerTest(fixture), IClassFixture<Db2WaitForDatabaseFixture>;
}