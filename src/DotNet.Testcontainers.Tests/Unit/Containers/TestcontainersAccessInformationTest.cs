namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Client;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using Xunit;

  public static class TestcontainersAccessInformationTest
  {
    public class AccessDockerInformation
    {
      [Fact]
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await DockerApiClientImage.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await DockerApiClientContainer.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await DockerApiClientImage.Instance.ExistsWithNameAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        Assert.False(await DockerApiClientContainer.Instance.ExistsWithNameAsync(string.Empty));
      }

      [Fact]
      public async Task QueryContainerInformationOfRunningContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          await testcontainer.StartAsync();

          Assert.NotEmpty(testcontainer.Name);
          Assert.NotEmpty(testcontainer.IpAddress);
          Assert.NotEmpty(testcontainer.MacAddress);
        }
      }

      [Fact]
      public void QueryContainerInformationOfStoppedContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
          Assert.Throws<InvalidOperationException>(() => testcontainer.IpAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.MacAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.GetMappedPublicPort(0));
        }
      }
    }
  }
}
