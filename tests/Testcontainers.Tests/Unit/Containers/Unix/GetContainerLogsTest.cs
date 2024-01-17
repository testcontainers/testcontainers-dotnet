namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class GetContainerLogsTest : IAsyncLifetime
  {
    private readonly IContainer _container = new ContainerBuilder()
      .WithImage("amazon/dynamodb-local:1.20.0")
      .WithWaitStrategy(Wait.ForUnixContainer()
        .UntilPortIsAvailable(8000))
      .Build();

    [Fact]
    public async Task GetLogsShouldNotBeEmpty()
    {
      var (stdout, _) = await _container.GetLogsAsync()
        .ConfigureAwait(true);

      Assert.NotEmpty(stdout);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenSinceIsOutOfDateRage()
    {
      var (stdout, stderr) = await _container.GetLogsAsync(since: DateTime.Now.AddDays(1))
        .ConfigureAwait(true);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenUntilIsOutOfDateRage()
    {
      var (stdout, stderr) = await _container.GetLogsAsync(until: DateTime.Now.AddDays(-1))
        .ConfigureAwait(true);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    public Task InitializeAsync()
    {
      return _container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return _container.DisposeAsync().AsTask();
    }
  }
}
