namespace DotNet.Testcontainers.Tests.Unit.Containers
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Clients;
  using DotNet.Testcontainers.Containers;
  using DotNet.Testcontainers.Containers.Builders;
  using DotNet.Testcontainers.Containers.Modules;
  using Xunit;

  public static class TestcontainersAccessInformationTest
  {
    public class AccessDockerInformation
    {
      private const string DoesNotExist = nameof(TestcontainersAccessInformationTest);

      [Fact]
      public async Task QueryNotExistingDockerImageById()
      {
        Assert.False(await new DockerImageOperations(DockerApiEndpoint.Local).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerById()
      {
        Assert.False(await new DockerContainerOperations(DockerApiEndpoint.Local).ExistsWithIdAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerImageByName()
      {
        Assert.False(await new DockerImageOperations(DockerApiEndpoint.Local).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryNotExistingDockerContainerByName()
      {
        Assert.False(await new DockerContainerOperations(DockerApiEndpoint.Local).ExistsWithNameAsync(DoesNotExist));
      }

      [Fact]
      public async Task QueryContainerInformationOfCreatedContainer()
      {
        // Given
        var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
          .WithImage("nginx");

        // When
        // Then
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
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
        await using (IDockerContainer testcontainer = testcontainersBuilder.Build())
        {
          Assert.Throws<InvalidOperationException>(() => testcontainer.Name);
          Assert.Throws<InvalidOperationException>(() => testcontainer.IpAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.MacAddress);
          Assert.Throws<InvalidOperationException>(() => testcontainer.GetMappedPublicPort(0));
          await Assert.ThrowsAsync<InvalidOperationException>(() => testcontainer.StopAsync());
        }
      }
    }
  }
}
