namespace Testcontainers.SqlEdge;

public sealed class SqlEdgeContainerTest : IAsyncLifetime
{
    private readonly SqlEdgeContainer _sqlEdgeContainer = new SqlEdgeBuilder().Build();

    public Task InitializeAsync()
    {
        return _sqlEdgeContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _sqlEdgeContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new SqlConnection(_sqlEdgeContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }
}