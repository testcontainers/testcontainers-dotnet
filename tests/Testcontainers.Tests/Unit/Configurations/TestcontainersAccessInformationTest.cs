namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Configurations;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public static class TestcontainersAccessInformationTest
  {
    [Collection(nameof(Testcontainers))]
    public sealed class AccessDockerInformation
    {
      private const string DoesNotExist = nameof(TestcontainersAccessInformationTest);

      [Fact]
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await new DockerImageOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await new DockerContainerOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerNetworkById()
      {
        Assert.False(await new DockerNetworkOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await new DockerImageOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        Assert.False(await new DockerContainerOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerNetworkByName()
      {
        Assert.False(await new DockerNetworkOperations(Guid.Empty, TestcontainersSettings.OS.DockerEndpointAuthConfig, TestcontainersSettings.Logger).ExistsWithNameAsync(DoesNotExist));
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
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
          Assert.Throws<InvalidOperationException>(() => testcontainer.IpAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.MacAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.GetMappedPublicPort(0));
          await Assert.ThrowsAsync<InvalidOperationException>(() => testcontainer.StopAsync());
        }
      }

      [Fact]
      public void QueryNetworkInformationOfNotCreatedNetwork()
      {
        // Given
        var networkBuilder = new TestcontainersNetworkBuilder();

        // When
        var network = networkBuilder.Build();

        // Then
        Assert.Throws<InvalidOperationException>(() => network.Id);
        Assert.Throws<InvalidOperationException>(() => network.Name);
      }
    }
  }
}
