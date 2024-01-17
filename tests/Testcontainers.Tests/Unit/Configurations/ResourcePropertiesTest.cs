namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class ResourcePropertiesTest
  {
    private static readonly string ResourceIdOrName = Guid.NewGuid().ToString("D");

    private static readonly ITestcontainersClient Client = new TestcontainersClient(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance);

    [Fact]
    public async Task QueryNotExistingDockerContainerById()
    {
      Assert.False(await Client.Container.ExistsWithIdAsync(ResourceIdOrName)
        .ConfigureAwait(true));
    }

    [Fact]
    public async Task QueryNotExistingDockerImageById()
    {
      Assert.False(await Client.Image.ExistsWithIdAsync(ResourceIdOrName)
        .ConfigureAwait(true));
    }

    [Fact]
    public async Task QueryNotExistingDockerNetworkById()
    {
      Assert.False(await Client.Network.ExistsWithIdAsync(ResourceIdOrName)
        .ConfigureAwait(true));
    }

    [Fact]
    public async Task QueryNotExistingDockerVolumeById()
    {
      Assert.False(await Client.Volume.ExistsWithIdAsync(ResourceIdOrName)
        .ConfigureAwait(true));
    }

    [Fact]
    public async Task QueryContainerInformationOfCreatedContainer()
    {
      // Given
      var container = new ContainerBuilder()
        .WithImage(CommonImages.Nginx)
        .Build();

      // When
      await container.StartAsync()
        .ConfigureAwait(true);

      // Then
      Assert.NotEmpty(container.Id);
      Assert.NotEmpty(container.Name);
      Assert.NotEmpty(container.IpAddress);
      Assert.NotEmpty(container.MacAddress);
      Assert.NotEmpty(container.Hostname);
    }

    [Fact]
    public async Task QueryContainerInformationOfNotCreatedContainer()
    {
      // Given
      var container = new ContainerBuilder()
        .WithImage(CommonImages.Nginx)
        .Build();

      // When
      await Task.CompletedTask
        .ConfigureAwait(true);

      // Then
      Assert.Throws<InvalidOperationException>(() => container.Id);
      Assert.Throws<InvalidOperationException>(() => container.Name);
      Assert.Throws<InvalidOperationException>(() => container.IpAddress);
      Assert.Throws<InvalidOperationException>(() => container.MacAddress);
      Assert.Throws<InvalidOperationException>(() => container.GetMappedPublicPort(0));
      Assert.Equal(TestcontainersStates.Undefined, container.State);
      Assert.Equal(TestcontainersHealthStatus.Undefined, container.Health);
    }

    [Fact]
    public async Task QueryImageInformationOfNotCreatedImage()
    {
      // Given
      var image = new ImageFromDockerfileBuilder()
        .Build();

      // When
      await Task.CompletedTask
        .ConfigureAwait(true);

      // Then
      Assert.Throws<InvalidOperationException>(() => image.Repository);
      Assert.Throws<InvalidOperationException>(() => image.Name);
      Assert.Throws<InvalidOperationException>(() => image.Tag);
      Assert.Throws<InvalidOperationException>(() => image.FullName);
      Assert.Throws<InvalidOperationException>(() => image.GetHostname());
    }

    [Fact]
    public async Task QueryNetworkInformationOfNotCreatedNetwork()
    {
      // Given
      var network = new NetworkBuilder().Build();

      // When
      await Task.CompletedTask
        .ConfigureAwait(true);

      // Then
      Assert.Throws<InvalidOperationException>(() => network.Name);
    }

    [Fact]
    public async Task QueryVolumeInformationOfNotCreatedVolume()
    {
      // Given
      var volume = new VolumeBuilder().Build();

      // When
      await Task.CompletedTask
        .ConfigureAwait(true);

      // Then
      Assert.Throws<InvalidOperationException>(() => volume.Name);
    }
  }
}
