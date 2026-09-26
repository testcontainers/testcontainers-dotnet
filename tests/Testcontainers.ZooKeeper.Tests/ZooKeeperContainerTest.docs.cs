namespace Testcontainers.ZooKeeper;

// <!-- -8<- [start:UseZooKeeperContainer] -->
public sealed class ZooKeeperContainerExample : IAsyncLifetime
{
    private readonly ZooKeeperContainer _zooKeeperContainer = new ZooKeeperBuilder(TestSession.GetImageFromDockerfile()).Build();

    public async ValueTask InitializeAsync()
    {
        await _zooKeeperContainer.StartAsync(TestContext.Current.CancellationToken)
            .ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        await _zooKeeperContainer.DisposeAsync()
            .ConfigureAwait(false);
    }
    // <!-- -8<- [end:UseZooKeeperContainer] -->

    // <!-- -8<- [start:EstablishConnection] -->
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetConnectionStringReturnsHostAndPort()
    {
        // Given
        var connectionString = _zooKeeperContainer.GetConnectionString();

        // Then
        Assert.Equal($"{_zooKeeperContainer.Hostname}:{_zooKeeperContainer.GetMappedPublicPort(ZooKeeperBuilder.ZooKeeperPort)}", connectionString);
    }
    // <!-- -8<- [end:EstablishConnection] -->
}
