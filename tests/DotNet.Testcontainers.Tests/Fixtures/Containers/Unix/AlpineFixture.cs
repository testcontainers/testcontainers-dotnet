namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;

  public sealed class AlpineFixture : ModuleFixture<TestcontainersContainer>
  {
    public AlpineFixture()
      : base(new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithCommand(KeepTestcontainersUpAndRunning.Command)
        .WithStartupCallback((_, ct) => Task.Delay(TimeSpan.FromMinutes(1), ct))
        .Build())
    {
    }

    public override Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public override Task DisposeAsync()
    {
      return this.Container.DisposeAsync().AsTask();
    }
  }
}
