namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using Xunit;

  public class ImageFromDockerfileTest
  {
    [Fact]
    public async Task DockerfileDoesNotExist()
    {
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory(".")
          .Build());

      Assert.Equal($"Dockerfile does not exist in '{new DirectoryInfo(".").FullName}'.", exception.Message);
    }

    [Fact]
    public async Task DockerfileDirectoryDoesNotExist()
    {
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory("DoesNotExist")
          .Build());

      Assert.Equal($"Directory '{new DirectoryInfo("DoesNotExist").FullName}' does not exist.", exception.Message);
    }

    [Fact]
    public async Task SimpleDockerfile()
    {
      // Given
      var imageFromDockerfile = await new ImageFromDockerfileBuilder()
        .WithName("alpine:custom")
        .WithDockerfileDirectory("Assets")
        .WithDeleteIfExists(true)
        .Build();

      // When
      // Then
      Assert.NotEmpty(imageFromDockerfile);
    }
  }
}
