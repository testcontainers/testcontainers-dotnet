using System.Collections.Generic;

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
  public sealed class AlpineBuilderFixture : IAsyncLifetime
  {
    private readonly List<IContainer> _containers = [];

    public IContainer Container(Func<ContainerBuilder, ContainerBuilder> builder)
    {
      var containerBuilder = builder(new ContainerBuilder());

      var container = containerBuilder
        .WithImage(CommonImages.Alpine)
        .WithCommand(CommonCommands.SleepInfinity)
        .Build();

      _containers.Add(container);

      return container;
    }

    public ValueTask InitializeAsync()
    {
      return ValueTask.CompletedTask;
    }

    public async ValueTask DisposeAsync()
    {
      foreach (var container in _containers)
      {
        await container.DisposeAsync();
      }
    }
  }
}
