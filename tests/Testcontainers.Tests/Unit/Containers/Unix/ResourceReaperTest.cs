namespace DotNet.Testcontainers.Tests.Unit.Containers.Unix
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Tests.Fixtures;
  using Xunit;

  public sealed class ResourceReaperTest : IAsyncLifetime
  {
    private ResourceReaper resourceReaper;

    [Fact]
    public async Task DockerContainerShouldBeDisposedByResourceReaper()
    {
      // Given
      var containerName = $"testcontainers-container-{Guid.NewGuid():D}";

      await new TestcontainersBuilder<TestcontainersContainer>()
        .WithImage("alpine")
        .WithName(containerName)
        .WithCommand(KeepTestcontainersUpAndRunning.Command)
        .WithResourceReaperSessionId(this.resourceReaper.SessionId)
        .Build()
        .StartAsync()
        .ConfigureAwait(false);

      // When
      await this.resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.False(Docker.Exists(DockerResource.Container, containerName));
    }

    [Fact]
    public async Task DockerImageShouldBeDisposedByResourceReaper()
    {
      // Given
      var dockerImageFullName = await new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory("Assets")
        .WithResourceReaperSessionId(this.resourceReaper.SessionId)
        .Build()
        .ConfigureAwait(false);

      // When
      await this.resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.False(Docker.Exists(DockerResource.Image, dockerImageFullName));
    }

    [Fact]
    public async Task DockerNetworkShouldBeDisposedByResourceReaper()
    {
      // Given
      var networkName = $"testcontainers-network-{Guid.NewGuid():D}";

      await new TestcontainersNetworkBuilder()
        .WithName(networkName)
        .WithResourceReaperSessionId(this.resourceReaper.SessionId)
        .Build()
        .CreateAsync()
        .ConfigureAwait(false);

      // When
      await this.resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.False(Docker.Exists(DockerResource.Network, networkName));
    }

    [Fact]
    public async Task DockerVolumeShouldBeDisposedByResourceReaper()
    {
      // Given
      var volumeName = $"testcontainers-volume-{Guid.NewGuid():D}";

      await new TestcontainersVolumeBuilder()
        .WithName(volumeName)
        .WithResourceReaperSessionId(this.resourceReaper.SessionId)
        .Build()
        .CreateAsync()
        .ConfigureAwait(false);

      // When
      await this.resourceReaper.DisposeAsync()
        .ConfigureAwait(false);

      // Then
      Assert.False(Docker.Exists(DockerResource.Volume, volumeName));
    }

    public async Task InitializeAsync()
    {
      this.resourceReaper = await ResourceReaper.GetAndStartNewAsync()
        .ConfigureAwait(false);
    }

    public Task DisposeAsync()
    {
      return this.resourceReaper.DisposeAsync().AsTask();
    }
  }
}
