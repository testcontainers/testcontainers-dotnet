namespace Testcontainers.ZooKeeper;

public sealed class ZooKeeperContainerTest(ZooKeeperContainerTest.ZooKeeperFixture fixture)
    : IClassFixture<ZooKeeperContainerTest.ZooKeeperFixture>
{
    /// <summary>
    /// Verifies that the connection string is the host:port pair ZooKeeper clients expect.
    /// </summary>
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetConnectionStringReturnsHostAndPort()
    {
        Assert.Equal($"{fixture.Container.Hostname}:{fixture.Container.GetMappedPublicPort(ZooKeeperBuilder.ZooKeeperPort)}", fixture.Container.GetConnectionString());
    }

    /// <summary>
    /// Verifies that a znode can be created and read back through the ZooKeeper CLI.
    /// </summary>
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task CreateAndReadZNodeReturnsValue()
    {
        // Given
        const string zNodePath = "/testcontainers";

        const string zNodeData = "hello";

        // When
        var createResult = await fixture.Container.ExecAsync(new[] { "zkCli.sh", "-server", "localhost:2181", "create", zNodePath, zNodeData }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        var getResult = await fixture.Container.ExecAsync(new[] { "zkCli.sh", "-server", "localhost:2181", "get", zNodePath }, TestContext.Current.CancellationToken)
            .ConfigureAwait(true);

        // Then
        Assert.True(0L.Equals(createResult.ExitCode), createResult.Stderr);
        Assert.True(0L.Equals(getResult.ExitCode), getResult.Stderr);
        Assert.Contains(zNodeData, getResult.Stdout);
    }

    /// <summary>
    /// Fixture that shares a single ZooKeeper container instance across the tests.
    /// </summary>
    [UsedImplicitly]
    public class ZooKeeperFixture(IMessageSink messageSink)
        : ContainerFixture<ZooKeeperBuilder, ZooKeeperContainer>(messageSink)
    {
        /// <inheritdoc />
        protected override ZooKeeperBuilder Configure()
            => new ZooKeeperBuilder(TestSession.GetImageFromDockerfile());
    }
}
