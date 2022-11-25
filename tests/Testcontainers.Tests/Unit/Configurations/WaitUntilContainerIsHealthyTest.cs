namespace DotNet.Testcontainers.Tests.Unit.Configurations
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Configurations.WaitStrategies;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class WaitUntilContainerIsHealthyTest
  {
    [Fact]
    public async Task StartsOnceHealthy()
    {
      var imageName = await new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory(Path.Combine(Environment.CurrentDirectory, "Assets", "healthWaitStrategy", "ok"))
        .WithDeleteIfExists(true)
        .Build();

      var container = new TestcontainersBuilder<TestcontainersContainer>()
       .WithImage(imageName)
       .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
       .Build();

      await container.StartAsync();
    }

    [Fact]
    public async Task ContainerStartFailsIfContainerIsUnhealthy()
    {
      var imageName = await new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory(Path.Combine(Environment.CurrentDirectory, "Assets", "healthWaitStrategy", "fail"))
        .WithDeleteIfExists(true)
        .Build();

      var container = new TestcontainersBuilder<TestcontainersContainer>()
       .WithImage(imageName)
       .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy())
       .Build();

      await Assert.ThrowsAsync<ContainerDidNotStartException>(async () => await container.StartAsync());
    }
  }
}
