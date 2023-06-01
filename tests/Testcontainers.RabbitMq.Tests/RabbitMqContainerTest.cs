namespace Testcontainers.RabbitMq;

public sealed class RabbitMqContainerTest : ContainerTest<RabbitMqBuilder, RabbitMqContainer>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void IsOpenReturnsTrue()
    {
        // Given
        var connectionFactory = new ConnectionFactory();
        connectionFactory.Uri = new Uri(Container.GetConnectionString());

        // When
        using var connection = connectionFactory.CreateConnection();

        // Then
        Assert.True(connection.IsOpen);
    }
}