namespace DotNet.Testcontainers.Tests.Fixtures
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using JetBrains.Annotations;
  using Xunit;

  [UsedImplicitly]
  public sealed class AlpineFixture : IAsyncLifetime
  {
    public IContainer Container { get; }
      = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithCommand(CommonCommands.SleepInfinity)
        .WithStartupCallback((_, ct) => Task.Delay(TimeSpan.FromMinutes(1), ct))
        .Build();

    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
      return Container.DisposeAsync().AsTask();
    }
  }
}
