namespace Testcontainers.ClickHouse;

public partial class ClickHouseContainerTest
{
    [UsedImplicitly]
    // <!-- -8<- [start:Class] -->
    public sealed class ClickHouseContainerTestDocumentation : IAsyncLifetime
    {
        private readonly ClickHouseContainer _clickHouseContainer = new ClickHouseBuilder().Build();

        public async ValueTask DisposeAsync()
        {
            await _clickHouseContainer.DisposeAsync();
        }

        public async ValueTask InitializeAsync()
        {
            await _clickHouseContainer.StartAsync(TestContext.Current.CancellationToken);
        }
        // <!-- -8<- [end:Class] -->

        // <!-- -8<- [start:Connecting] -->
        [Fact]
        [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
        public void ConnectionStateReturnsOpen()
        {
            // Given
            using DbConnection connection = new ClickHouseConnection(_clickHouseContainer.GetConnectionString());

            // When
            connection.Open();

            // Then
            Assert.Equal(ConnectionState.Open, connection.State);
        }
        // <!-- -8<- [end:Connecting] -->

        // <!-- -8<- [start:SQLScript] -->
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
        // <!-- -8<- [end:SQLScript] -->
    }
}