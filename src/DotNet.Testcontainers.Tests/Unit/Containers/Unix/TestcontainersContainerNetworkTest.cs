namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Tests.Fixtures;
  using DotNet.Testcontainers.Tests.Fixtures.Networks;
  using Xunit;

  public class TestcontainersContainerNetworkTest : IClassFixture<NetworkFixture>, IAsyncLifetime
  {
    private readonly IDockerContainer testcontainer1;

    private readonly IDockerContainer testcontainer2;

    public TestcontainersContainerNetworkTest(NetworkFixture networkFixture)
    {
      var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithEntrypoint(KeepTestcontainersUpAndRunning.Command)
        .WithNetwork(networkFixture.Network.Id, networkFixture.Network.Name);

      this.testcontainer1 = testcontainersBuilder
        .WithHostname(nameof(this.testcontainer1))
        .Build();

      this.testcontainer2 = testcontainersBuilder
        .WithHostname(nameof(this.testcontainer2))
        .Build();
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(this.testcontainer1.StartAsync(), this.testcontainer2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.WhenAll(this.testcontainer1.DisposeAsync().AsTask(), this.testcontainer2.DisposeAsync().AsTask());
    }

    [Fact]
    public async Task PingContainer()
    {
      Assert.Equal(0, await this.testcontainer1.ExecAsync(new[] { "ping", "-c", "4", nameof(this.testcontainer2) }));
    }
  }
}
