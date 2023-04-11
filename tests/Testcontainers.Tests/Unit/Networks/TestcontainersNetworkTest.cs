namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class TestcontainersNetworkTest : IClassFixture<NetworkFixture>, IAsyncLifetime
  {
    private const string AliasSuffix = "-alias";

    private readonly IContainer _testcontainer1;

    private readonly IContainer _testcontainer2;

    public TestcontainersNetworkTest(NetworkFixture networkFixture)
    {
      var testcontainersBuilder = new ContainerBuilder()
        .WithImage("alpine")
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .WithNetwork(networkFixture.Network.Name);

      _testcontainer1 = testcontainersBuilder
        .WithHostname(nameof(_testcontainer1))
        .WithNetworkAliases(nameof(_testcontainer1) + AliasSuffix)
        .Build();

      _testcontainer2 = testcontainersBuilder
        .WithHostname(nameof(_testcontainer2))
        .WithNetworkAliases(nameof(_testcontainer2) + AliasSuffix)
        .Build();
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(_testcontainer1.StartAsync(), _testcontainer2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.WhenAll(_testcontainer1.DisposeAsync().AsTask(), _testcontainer2.DisposeAsync().AsTask());
    }

    [Theory]
    [InlineData(nameof(_testcontainer2))]
    [InlineData(nameof(_testcontainer2) + AliasSuffix)]
    public async Task PingContainer(string destination)
    {
      var execResult = await _testcontainer1.ExecAsync(new[] { "ping", "-c", "4", destination });
      Assert.Equal(0, execResult.ExitCode);
    }
  }
}
