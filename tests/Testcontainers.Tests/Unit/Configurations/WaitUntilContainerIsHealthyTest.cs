namespace DotNet.Testcontainers.Tests.Unit
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Images;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class WaitUntilContainerIsHealthyTest : IClassFixture<HealthCheckFixture>
  {
    private readonly IImage _image;

    public WaitUntilContainerIsHealthyTest(HealthCheckFixture image)
    {
      _image = image;
    }

    [Fact]
    public async Task ContainerHealthCheckShouldBeHealthy()
    {
      // Given
      var container = new ContainerBuilder()
        .WithImage(_image)
        .WithEnvironment("START_HEALTHY", bool.TrueString.ToLowerInvariant())
        .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy(10))
        .Build();

      // When
      await container.StartAsync()
        .ConfigureAwait(true);

      // Then
      Assert.Equal(TestcontainersHealthStatus.Healthy, container.Health);
    }

    [Fact]
    public async Task ContainerHealthCheckShouldBeUnhealthy()
    {
      // Given
      var container = new ContainerBuilder()
        .WithImage(_image)
        .WithEnvironment("START_HEALTHY", bool.FalseString.ToLowerInvariant())
        .WithWaitStrategy(Wait.ForUnixContainer().UntilContainerIsHealthy(10))
        .Build();

      // When
      _ = await Record.ExceptionAsync(() => container.StartAsync())
        .ConfigureAwait(true);

      // Then
      Assert.Equal(TestcontainersHealthStatus.Unhealthy, container.Health);
    }
  }
}
