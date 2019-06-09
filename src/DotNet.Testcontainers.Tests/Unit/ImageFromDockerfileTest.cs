namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core;
  using DotNet.Testcontainers.Core.Builder;
  using Xunit;

  public class ImageFromDockerfileTest
  {
    [Fact]
    public void DockerfileArchiveTar()
    {
      // Given
      var dockerFileArchive = new DockerfileArchive("./Assets");

      using (var file = new FileInfo(dockerFileArchive.Tar()).OpenRead())
      {
        Assert.Equal("34BDCA7AB58F0BBDB64AA0178D69CD6E14586838", BitConverter.ToString(System.Security.Cryptography.SHA1.Create().ComputeHash(file)).Replace("-", string.Empty));
      }
    }

    [Fact]
    public async Task DockerfileDoesNotExist()
    {
      // Given
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory(".")
          .Build());

      // When
      // Then
      Assert.Equal($"Dockerfile does not exist in '{new DirectoryInfo(".").FullName}'.", exception.Message);
    }

    [Fact]
    public async Task DockerfileDirectoryDoesNotExist()
    {
      // Given
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory("DoesNotExist")
          .Build());

      // When
      // Then
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
