namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using DotNet.Testcontainers.Containers.WaitStrategies;
  using Xunit;

  public static class TestcontainersContainerCancellationTest
  {
    public class Cancel
    {
      [Fact]
      public async Task Start()
      {
        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
        {
          await using (IDockerContainer testcontainer = new TestcontainersBuilder<TestcontainersContainer>()
            .WithImage("alpine")
            .WithWaitStrategy(Wait.ForUnixContainer()
              .AddCustomWaitStrategy(new TestcontainersWaitStrategyTest.Timeout()))
            .Build())
          {
            await Assert.ThrowsAsync<TaskCanceledException>(() => testcontainer.StartAsync(cts.Token));
          }
        }
      }
    }
  }
}
