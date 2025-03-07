namespace Testcontainers.PostgreSql;

public abstract class PostgreSqlContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UsePostgreSqlContainer]
    private readonly PostgreSqlContainer _postgreSqlContainer;

    public PostgreSqlContainer Container { get { return _postgreSqlContainer; } }

    public PostgreSqlContainerTest(PostgreSqlContainer postgreSqlContainer)
    {
        _postgreSqlContainer = postgreSqlContainer;
    }

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

    public sealed class ReusePostgres15ContainerTest : IClassFixture<PostgreSql15Fixture>, IDisposable
    {
        private readonly CancellationTokenSource _cts = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        private readonly PostgreSql15Fixture _fixture;

        public ReusePostgres15ContainerTest(PostgreSql15Fixture fixture)
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
    public sealed class PostgreSql15Fixture : PostgreSqlContainerTest
    {
        public PostgreSql15Fixture()
            : base(new PostgreSqlBuilder().Build())
        {
        }
    }

    [UsedImplicitly]
    public sealed class PostgreSql9Fixture : PostgreSqlContainerTest
    {
        // https://github.com/testcontainers/testcontainers-dotnet/issues/1142.
        public PostgreSql9Fixture()
            : base(new PostgreSqlBuilder().WithImage("postgres:9.2").Build())
        {
        }
    }
}