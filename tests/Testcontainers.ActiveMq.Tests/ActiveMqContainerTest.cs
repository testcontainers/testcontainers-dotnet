namespace Testcontainers.ActiveMq;

public sealed class ActiveMqContainerTest : IAsyncLifetime
{
    private readonly ActiveMqContainer _activeMqContainer = new ActiveMqBuilder().Build();

    public Task InitializeAsync()
    {
        return _activeMqContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _activeMqContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ConnectionIsOpen()
    {
        // Given
        var factory = new ConnectionFactory(_activeMqContainer.GetBrokerAddress());

        // When
        var connection = await factory.CreateConnectionAsync(ActiveMqBuilder.DefaultUsername, ActiveMqBuilder.DefaultPassword);
        await connection.StartAsync();

        // Then
        Assert.True(connection.IsStarted);
    }
}