namespace Testcontainers.ClickHouse;

public abstract partial class ClickHouseContainerTest
{
    // <!-- -8<- [start:UseClickHouseContainer] -->
    public sealed class ClickHouseContainerExample : IAsyncLifetime
    {
        private readonly ClickHouseContainer _clickHouseContainer = new ClickHouseBuilder().Build();

        public async ValueTask InitializeAsync()
        {
            await _clickHouseContainer
                .StartAsync(TestContext.Current.CancellationToken)
                .ConfigureAwait(false);
        }

        public async ValueTask DisposeAsync()
        {
            await _clickHouseContainer.DisposeAsync().ConfigureAwait(false);
        }

        // <!-- -8<- [end:UseClickHouseContainer] -->

        // <!-- -8<- [start:EstablishConnection] -->
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void ConnectionStateReturnsOpen()
        {
            // Given
            using DbConnection connection = new ClickHouseConnection(
                _clickHouseContainer.GetConnectionString()
            );

            // When
            connection.Open();

            // Then
            Assert.Equal(ConnectionState.Open, connection.State);
        }

        // <!-- -8<- [end:EstablishConnection] -->

        // <!-- -8<- [start:RunSQLScript] -->
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public async Task ExecScriptReturnsSuccessful()
        {
            // Given
            const string scriptContent = "SELECT 1;";

            // When
            var execResult = await _clickHouseContainer
                .ExecScriptAsync(scriptContent, TestContext.Current.CancellationToken)
                .ConfigureAwait(true);

            // Then
            Assert.True(0L.Equals(execResult.ExitCode), execResult.Stderr);
            Assert.Empty(execResult.Stderr);
        }
        // <!-- -8<- [end:RunSQLScript] -->
    }
}
