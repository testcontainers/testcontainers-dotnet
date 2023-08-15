namespace Testcontainers.InfluxDb.Tests;

public sealed class InfluxDbContainerTest : IAsyncLifetime
{
    private readonly InfluxDbContainer _influxDbContainer = new InfluxDbBuilder().Build();

    public Task InitializeAsync()
    {
        return _influxDbContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _influxDbContainer.DisposeAsync().AsTask();
    }

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task PingReturnsTrue()
    {
        // Given
        using var client = new InfluxDBClient(_influxDbContainer.GetAddress());

        // When
        var result = await client.PingAsync();

        // Then
        Assert.True(result);
    }
}