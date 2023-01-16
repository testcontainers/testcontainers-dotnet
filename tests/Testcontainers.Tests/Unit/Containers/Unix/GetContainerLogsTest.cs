namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class GetContainerLogsTest : IAsyncLifetime
  {
    private readonly ITestcontainersContainer container = new TestcontainersBuilder<TestcontainersContainer>()
      .WithImage("amazon/dynamodb-local:1.20.0")
      .WithWaitStrategy(Wait.ForUnixContainer()
        .UntilPortIsAvailable(8000))
      .Build();

    [Fact]
    public async Task GetLogsShouldNotBeEmpty()
    {
      var (stdout, _) = await this.container.GetLogs()
        .ConfigureAwait(false);

      Assert.NotEmpty(stdout);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenSinceIsOutOfDateRage()
    {
      var (stdout, stderr) = await this.container.GetLogs(since: DateTime.Now.AddDays(1))
        .ConfigureAwait(false);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    [Fact]
    public async Task GetLogsShouldBeEmptyWhenUntilIsOutOfDateRage()
    {
      var (stdout, stderr) = await this.container.GetLogs(until: DateTime.Now.AddDays(-1))
        .ConfigureAwait(false);

      Assert.Empty(stdout);
      Assert.Empty(stderr);
    }

    public Task InitializeAsync()
    {
      return this.container.StartAsync();
    }

    public Task DisposeAsync()
    {
      return this.container.DisposeAsync().AsTask();
    }
  }
}
