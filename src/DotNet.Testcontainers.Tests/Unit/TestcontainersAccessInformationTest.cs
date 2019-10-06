namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Builder;
  using DotNet.Testcontainers.Core.Containers;
  using Xunit;

  public static class TestcontainersAccessInformationTest
  {
    public class AccessDockerInformation
    {
      [Fact]
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await MetaDataClientImages.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await MetaDataClientContainers.Instance.ExistsWithIdAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await MetaDataClientImages.Instance.ExistsWithNameAsync(string.Empty));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        Assert.False(await MetaDataClientContainers.Instance.ExistsWithNameAsync(string.Empty));
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
          Assert.NotEmpty(testcontainer.IPAddress);
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
          Assert.Throws<InvalidOperationException>(() => testcontainer.IPAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.MacAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.GetMappedPublicPort(0));
        }
      }
    }
  }
}
