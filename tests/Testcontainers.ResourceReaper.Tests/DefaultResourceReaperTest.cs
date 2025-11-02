namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class DefaultResourceReaperTest : IAsyncLifetime
  {
    public async ValueTask InitializeAsync()
    {
      var resourceReaper = await ResourceReaper.GetAndStartDefaultAsync(TestcontainersSettings.OS.DockerEndpointAuthConfig, ConsoleLogger.Instance)
        .ConfigureAwait(false);

      await resourceReaper.DisposeAsync()
        .ConfigureAwait(false);
    }

    public ValueTask DisposeAsync()
    {
      return ValueTask.CompletedTask;
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ContainerCleanUpStartsDefaultResourceReaper(bool resourceReaperEnabled)
    {
      // Given
      var container = new ContainerBuilder()
        .WithImage(CommonImages.Alpine)
        .WithEntrypoint(CommonCommands.SleepInfinity)
        .WithAutoRemove(true)
        .WithCleanUp(resourceReaperEnabled)
        .Build();

      // When
      await container.StartAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      await container.StopAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Equal(resourceReaperEnabled, DockerCli.ResourceExists(DockerCli.DockerResource.Container, "testcontainers-ryuk-" + ResourceReaper.DefaultSessionId.ToString("D")));
    }
  }
}
