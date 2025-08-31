namespace Testcontainers.RabbitMq;

public sealed class RabbitMqContainerTest : IAsyncLifetime
{
    // # --8<-- [start:UseRabbitMqContainer]
    private readonly RabbitMqContainer _rabbitMqContainer = new RabbitMqBuilder().Build();

    public async ValueTask InitializeAsync()
    {
        await _rabbitMqContainer.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
        return _rabbitMqContainer.DisposeAsync();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void IsOpenReturnsTrue()
    {
        // Given
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(_rabbitMqContainer.GetConnectionString());

        // When
        using var connection = connectionFactory.CreateConnection();

        // Then
        Assert.True(connection.IsOpen);
    }
    // # --8<-- [end:UseRabbitMqContainer]
}
