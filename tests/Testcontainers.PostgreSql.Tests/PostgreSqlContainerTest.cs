namespace Testcontainers.PostgreSql;

public abstract class PostgreSqlContainerTest(ITestOutputHelper testOutputHelper) : DbContainerTest<PostgreSqlBuilder, PostgreSqlContainer>(testOutputHelper)
{
    public override DbProviderFactory DbProviderFactory => NpgsqlFactory.Instance;

    // # --8<-- [start:UsePostgreSqlContainer]
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = CreateConnection();

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
        const string scriptContent = "SELECT 1;";

        // When
        var execResult = await Container.ExecScriptAsync(scriptContent)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
        Assert.Empty(execResult.Stderr);
    }
    // # --8<-- [end:UsePostgreSqlContainer]

    public sealed class ReuseContainerTest : IClassFixture<PostgreSqlFixture>, IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        private readonly PostgreSqlFixture _fixture;

        public ReuseContainerTest(PostgreSqlFixture fixture)
        {
            _fixture = fixture;
        }

        public void Dispose()
        {
            _cts.Dispose();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public async Task StopsAndStartsContainerSuccessful(int _)
        {
            await _fixture.Container.StopAsync(_cts.Token)
                .ConfigureAwait(true);

            await _fixture.Container.StartAsync(_cts.Token)
                .ConfigureAwait(true);

            Assert.False(_cts.IsCancellationRequested);
        }
    }

    [UsedImplicitly]
    public sealed class PostgreSqlFixture : ContainerFixture<PostgreSqlBuilder, PostgreSqlContainer>
    {
        public PostgreSqlFixture(IMessageSink messageSink)
            : base(messageSink)
        {
        }
    }

    [UsedImplicitly]
    public sealed class PostgreSqlDefaultConfiguration(ITestOutputHelper testOutputHelper) : PostgreSqlContainerTest(testOutputHelper);

    [UsedImplicitly]
    public sealed class PostgreSqlWaitForDatabase(ITestOutputHelper testOutputHelper) : PostgreSqlContainerTest(testOutputHelper)
    {
        protected override PostgreSqlBuilder Configure(PostgreSqlBuilder builder)
        {
            return builder.WithWaitStrategy(Wait.ForUnixContainer().UntilDatabaseIsAvailable(DbProviderFactory));
        }
    }
}