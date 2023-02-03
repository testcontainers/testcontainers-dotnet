namespace Testcontainers.MsSql.Tests;

public sealed class MsSqlContainerTest : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer = new MsSqlBuilder().Build();

    public Task InitializeAsync()
    {
        return _msSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _msSqlContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void ConnectionStateReturnsOpen()
    {
        // Given
        using DbConnection connection = new SqlConnection(_msSqlContainer.GetConnectionString());

        // When
        connection.Open();

        // Then
        Assert.Equal(ConnectionState.Open, connection.State);
    }
}