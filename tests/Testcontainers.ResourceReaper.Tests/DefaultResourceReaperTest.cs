namespace DotNet.Testcontainers.ResourceReaper.Tests
{
  using System;
  using System.Threading.Tasks;
  using Docker.DotNet.Models;
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
      var container = new ContainerBuilder(CommonImages.Alpine)
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

    [Fact]
    [Trait(nameof(DockerCli.DockerPlatform), nameof(DockerCli.DockerPlatform.Linux))]
    public async Task ClosedConnectionTerminatesResourceReaperConnection()
    {
      // Given
      var connectionTerminated = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

      EventHandler<ResourceReaperStateEventArgs> stateChanged = (_, e) =>
      {
        if (ResourceReaperState.ConnectionTerminated.Equals(e.State))
        {
          connectionTerminated.TrySetResult(true);
        }
      };

      using var dockerClient = TestcontainersSettings.OS.DockerEndpointAuthConfig.GetDockerClientBuilder(Guid.NewGuid()).Build();

      var resourceReaper = await ResourceReaper.GetAndStartDefaultAsync(TestcontainersSettings.OS.DockerEndpointAuthConfig, ConsoleLogger.Instance, ct: TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      ResourceReaper.StateChanged += stateChanged;

      try
      {
        // When
        await dockerClient.Containers.RemoveContainerAsync("testcontainers-ryuk-" + ResourceReaper.DefaultSessionId.ToString("D"), new ContainerRemoveParameters { Force = true }, TestContext.Current.CancellationToken)
          .ConfigureAwait(true);

        // Then
        Assert.True(await connectionTerminated.Task.WaitAsync(TimeSpan.FromSeconds(30), TestContext.Current.CancellationToken)
          .ConfigureAwait(true));
      }
      finally
      {
        ResourceReaper.StateChanged -= stateChanged;

        await resourceReaper.DisposeAsync()
          .ConfigureAwait(true);
      }
    }
  }
}
