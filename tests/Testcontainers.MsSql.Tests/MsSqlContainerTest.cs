namespace Testcontainers.MsSql;

public abstract class MsSqlContainerTest(MsSqlContainerTest.MsSqlDefaultFixture fixture)
{
    // # --8<-- [start:UseMsSqlContainer]
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
        Assert.Equal(fixture.Container.GetConnectionString(), fixture.Container.GetConnectionString(ConnectionMode.Host));
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ExecScriptReturnsSuccessful()
    {
        // Given
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UseMsSqlContainer]

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreatesTableInConfiguredDatabase()
    {
        // Given
        const string scriptContent = "CREATE TABLE dbo.Employee (EmployeeID INT PRIMARY KEY CLUSTERED);";

        using var command = fixture.CreateCommand("SELECT DB_NAME() FROM sys.Tables WHERE Name = 'Employee' AND SCHEMA_ID = SCHEMA_ID('dbo');");

        // When
        var execResult = await fixture.Container.ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var database = await command.ExecuteScalarAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Equal(fixture.Database, database);
    }

    public class MsSqlDefaultFixture(IMessageSink messageSink)
        : DbContainerFixture<MsSqlBuilder, MsSqlContainer>(messageSink)
    {
        protected override MsSqlBuilder Configure()
            => new MsSqlBuilder(TestSession.GetImageFromDockerfile());

        public override DbProviderFactory DbProviderFactory
            => SqlClientFactory.Instance;

        public virtual string Database
            => MsSqlBuilder.DefaultDatabase;
    }

    [UsedImplicitly]
    public class MsSqlWaitForDatabaseFixture(IMessageSink messageSink)
        : MsSqlDefaultFixture(messageSink)
    {
        protected override MsSqlBuilder Configure()
            => base.Configure().WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
    }

    [UsedImplicitly]
    public class MsSqlCustomDatabaseFixture(IMessageSink messageSink)
        : MsSqlDefaultFixture(messageSink)
    {
        protected override MsSqlBuilder Configure()
            => base.Configure().WithDatabase(Database);

        public override string Database
            => "MyDatabase";
    }

    [UsedImplicitly]
    public class MsSqlWaitForCustomDatabaseFixture(IMessageSink messageSink)
        : MsSqlDefaultFixture(messageSink)
    {
        protected override MsSqlBuilder Configure()
            => base.Configure().WithDatabase(Database).WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));

        public override string Database
            => "MyDatabase";
    }

    [UsedImplicitly]
    public sealed class MsSqlDefaultConfiguration(MsSqlDefaultFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlDefaultFixture>;

    [UsedImplicitly]
    public sealed class MsSqlWaitForDatabaseConfiguration(MsSqlWaitForDatabaseFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlWaitForDatabaseFixture>;

    [UsedImplicitly]
    public sealed class MsSqlCustomDatabaseConfiguration(MsSqlCustomDatabaseFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlCustomDatabaseFixture>;

    [UsedImplicitly]
    public sealed class MsSqlWaitForCustomDatabaseConfiguration(MsSqlWaitForCustomDatabaseFixture fixture)
        : MsSqlContainerTest(fixture), IClassFixture<MsSqlWaitForCustomDatabaseFixture>;
}