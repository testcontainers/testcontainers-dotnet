namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Core.Builder;
  using Xunit;

  public class TestcontainersAccessInformationTest
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
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("nginx");

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
        // When
        var testcontainersBuilder = new TestcontainersBuilder()
          .WithImage("nginx");

        // Then
        using (var testcontainer = testcontainersBuilder.Build())
        {
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
        }
      }
    }
  }
}
