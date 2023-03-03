namespace DotNet.Testcontainers.Tests.Unit.Configurations
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class WaitUntilContainerIsHealthyTest : IClassFixture<HealthCheckFixture>
  {
    private readonly IImage image;

    public WaitUntilContainerIsHealthyTest(HealthCheckFixture image)
    {
      this.image = image;
    }

    [Fact]
    public async Task ContainerHealthCheckShouldBeHealthy()
    {
      // Given
      var container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(this.image)
        .WithEnvironment("START_HEALTHY", bool.TrueString.ToLowerInvariant())
        .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy(10))
        .Build();

      // When
      await container.StartAsync()
        .ConfigureAwait(false);

      // Then
      Assert.Equal(TestcontainersHealthStatus.Healthy, container.Health);
    }

    [Fact]
    public async Task ContainerHealthCheckShouldBeUnhealthy()
    {
      // Given
      var container = new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage(this.image)
        .WithEnvironment("START_HEALTHY", bool.FalseString.ToLowerInvariant())
        .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy(10))
        .Build();

      // When
      _ = await Record.ExceptionAsync(() => container.StartAsync())
        .ConfigureAwait(false);

      // Then
      Assert.Equal(TestcontainersHealthStatus.Unhealthy, container.Health);
    }
  }
}
