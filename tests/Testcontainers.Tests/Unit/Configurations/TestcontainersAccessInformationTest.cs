namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public static class TestcontainersAccessInformationTest
  {
    public sealed class AccessDockerInformation
    {
      private const string DoesNotExist = nameof(TestcontainersAccessInformationTest);

      [Fact]
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await new DockerImageOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await new DockerContainerOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerNetworkById()
      {
        Assert.False(await new DockerNetworkOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await new DockerImageOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        Assert.False(await new DockerContainerOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerNetworkByName()
      {
        Assert.False(await new DockerNetworkOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, NullLogger.Instance).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryContainerInformationOfCreatedContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();

          Assert.NotEmpty(testcontainer.Id);
          Assert.NotEmpty(testcontainer.Name);
          Assert.NotEmpty(testcontainer.IpAddress);
          Assert.NotEmpty(testcontainer.MacAddress);
          Assert.NotEmpty(testcontainer.Hostname);
        }
      }

      [Fact]
      public async Task QueryContainerInformationOfNotCreatedContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        await using (ITestcontainersContainer testcontainer = testcontainersBuilder.Build())
        {
          Assert.Throws<InvalidOperationException>(() => testcontainer.Id);
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
          Assert.Throws<InvalidOperationException>(() => testcontainer.IpAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.MacAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.GetMappedPublicPort(0));
          Assert.Equal(TestcontainersStates.Undefined, testcontainer.State);
          Assert.Equal(TestcontainersHealthStatus.Undefined, testcontainer.Health);
        }
      }

      [Fact]
      public void QueryImageInformationOfNotCreatedImage()
      {
        // Given
        var imageBuilder = new ImageFromDockerfileBuilder();

        // When
        var image = imageBuilder.Build();

        // Then
        Assert.Throws<InvalidOperationException>(() => image.Repository);
        Assert.Throws<InvalidOperationException>(() => image.Name);
        Assert.Throws<InvalidOperationException>(() => image.Tag);
        Assert.Throws<InvalidOperationException>(() => image.FullName);
        Assert.Throws<InvalidOperationException>(() => image.GetHostname());
      }

      [Fact]
      public void QueryNetworkInformationOfNotCreatedNetwork()
      {
        // Given
        var networkBuilder = new NetworkBuilder()
          .WithName(Guid.NewGuid().ToString("D"));

        // When
        var network = networkBuilder.Build();

        // Then
        Assert.Throws<InvalidOperationException>(() => network.Name);
      }

      [Fact]
      public void QueryVolumeInformationOfNotCreatedVolume()
      {
        // Given
        var volumeBuilder = new VolumeBuilder()
          .WithName(Guid.NewGuid().ToString("D"));

        // When
        var volume = volumeBuilder.Build();

        // Then
        Assert.Throws<InvalidOperationException>(() => volume.Name);
      }
    }
  }
}
