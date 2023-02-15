namespace Testcontainers.RabbitMq;

public sealed class RabbitMqContainerTest : IAsyncLifetime
{
    private readonly RabbitMqContainer _rabbitMqContainerContainer = new RabbitMqBuilder().Build();

    public Task InitializeAsync()
    {
        return _rabbitMqContainerContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _rabbitMqContainerContainer.DisposeAsync().AsTask();
    }
}