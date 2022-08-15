namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  [Collection(nameof(Testcontainers))]
  public sealed class TestcontainersNetworkTest : IClassFixture<NetworkFixture>, IAsyncLifetime
  {
    private const string AliasSuffix = "-alias";

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
        .WithNetworkAliases(nameof(this.testcontainer1) + AliasSuffix)
        .Build();

      this.testcontainer2 = testcontainersBuilder
        .WithHostname(nameof(this.testcontainer2))
        .WithNetworkAliases(nameof(this.testcontainer2) + AliasSuffix)
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

    [Theory]
    [InlineData(nameof(testcontainer2))]
    [InlineData(nameof(testcontainer2) + AliasSuffix)]
    public async Task PingContainer(string destination)
    {
      var execResult = await this.testcontainer1.ExecAsync(new[] { "ping", "-c", "4", destination });
      Assert.Equal(0, execResult.ExitCode);
    }
  }
}
