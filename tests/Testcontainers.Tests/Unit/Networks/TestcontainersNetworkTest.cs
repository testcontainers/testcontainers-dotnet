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

    private readonly IContainer _container1;

    private readonly IContainer _container2;

    public TestcontainersNetworkTest(NetworkFixture networkFixture)
    {
      var containerBuilder = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .WithNetwork(networkFixture.Network.Name);

      _container1 = containerBuilder
        .WithNetworkAliases(nameof(_container1) + AliasSuffix)
        .Build();

      _container2 = containerBuilder
        .WithNetworkAliases(nameof(_container2) + AliasSuffix)
        .Build();
    }

    public Task InitializeAsync()
    {
      return Task.WhenAll(_container1.StartAsync(), _container2.StartAsync());
    }

    public Task DisposeAsync()
    {
      return Task.WhenAll(_container1.DisposeAsync().AsTask(), _container2.DisposeAsync().AsTask());
    }

    [Fact]
    public async Task PingContainer()
    {
      // Given
      const string destination = nameof(_container2) + AliasSuffix;

      // When
      var execResult = await _container1.ExecAsync(new[] { "ping", "-c", "1", destination })
        .ConfigureAwait(true);

      // Then
      Assert.Equal(0, execResult.ExitCode);
    }
  }
}
