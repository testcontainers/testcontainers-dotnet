namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class TestcontainersNetworkTest : IClassFixture<NetworkFixture>, IAsyncLifetime
  {
    private readonly ITestcontainersContainer testcontainer1;

    private readonly ITestcontainersContainer testcontainer2;

    public TestcontainersNetworkTest(NetworkFixture networkFixture)
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
      Assert.Equal(0, (await this.testcontainer1.ExecAsync(new[] { "ping", "-c", "4", nameof(this.testcontainer2) })).ExitCode);
    }
  }
}
