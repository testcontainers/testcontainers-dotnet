namespace Testcontainers.RabbitMq;

public sealed class RabbitMqContainerTest : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    public Task InitializeAsync()
    {
        return _rabbitMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync().AsTask();
    }

    [Fact]
    public void IsOpenReturnsOpen()
    {
        // Given
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());

        // When
        using var connection = connectionFactory.CreateConnection();

        // Then
        Assert.True(connection.IsOpen);
    }
}