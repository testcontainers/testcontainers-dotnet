namespace DotNet.Testcontainers.Tests.Fixtures.Containers.Modules
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using Xunit;

  public class AlpineFixture : ModuleFixture<TestcontainersContainer>, IAsyncLifetime
  {
    public AlpineFixture()
      : base(new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithCommand(KeepTestcontainersUpAndRunning.Command)
        .WithStartupCallback((_ , ct) => Task.Delay(TimeSpan.FromMinutes(1), ct))
        .Build())
    {
    }

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
