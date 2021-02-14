namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public class AlpineFixture : ModuleFixture<TestcontainersContainer>, IAsyncLifetime
  {
    public AlpineFixture()
      : base(new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithCommand(KeepTestcontainersUpAndRunning.Command)
        .WithWaitStrategy(Wait.ForUnixContainer()
          .UntilCommandIsCompleted("sleep 10"))
        .Build())
    {
    }

    public async Task InitializeAsync()
    {
      await this.Container.StartAsync()
        .ConfigureAwait(false);

      await this.Container.StopAsync()
        .ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
