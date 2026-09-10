namespace Testcontainers.ZooKeeper;

public sealed class ZooKeeperContainerTest(ZooKeeperContainerTest.ZooKeeperFixture fixture)
    : IClassFixture<ZooKeeperContainerTest.ZooKeeperFixture>
{
    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public void GetConnectionStringReturnsHostAndPort()
    {
        Assert.Equal($"{fixture.Container.Hostname}:{fixture.Container.GetMappedPublicPort(ZooKeeperBuilder.ZooKeeperPort)}", fixture.Container.GetConnectionString());
    }

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

    [UsedImplicitly]
    public class ZooKeeperFixture(IMessageSink messageSink)
        : ContainerFixture<ZooKeeperBuilder, ZooKeeperContainer>(messageSink)
    {
        protected override ZooKeeperBuilder Configure()
            => new ZooKeeperBuilder(TestSession.GetImageFromDockerfile());
    }
}
