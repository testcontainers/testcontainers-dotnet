namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class GetContainerLogsTest : IAsyncLifetime
  {
    private readonly IContainer _container = new ContainerBuilder("amazon/dynamodb-local:1.20.0")
      .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(8000))
      .Build();

    [Fact]
    public async Task GetLogsShouldNotBeEmpty()
    {
      var (stdout, _) = await _container.GetLogsAsync(ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.NotEmpty(stdout);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenSinceIsOutOfDateRage()
    {
      var (stdout, stderr) = await _container.GetLogsAsync(since: DateTime.Now.AddDays(1), ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenUntilIsOutOfDateRage()
    {
      var (stdout, stderr) = await _container.GetLogsAsync(until: DateTime.Now.AddDays(-1), ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    public async ValueTask InitializeAsync()
    {
      await _container.StartAsync()
        .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
      return _container.DisposeAsync();
    }
  }
}
