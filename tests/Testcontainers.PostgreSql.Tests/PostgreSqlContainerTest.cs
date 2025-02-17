namespace Testcontainers.PostgreSql;

public sealed class PostgreSqlContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UsePostgreSqlContainer]
    private readonly PostgreSqlContainer _postgreSqlContainer = new PostgreSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return _postgreSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgreSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new NpgsqlConnection(_postgreSqlContainer.GetConnectionString());

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
        var execResult = await _postgreSqlContainer.ExecScriptAsync(scriptContent)
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
}