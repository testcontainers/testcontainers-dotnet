namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Images;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class ImageFromDockerfileTest
  {
    [Fact]
    public void DockerfileArchiveTar()
    {
      // Given
      var image = new DockerImage("testcontainers", "test", "0.1.0");

      var expected = new SortedSet<string> { ".dockerignore", "Dockerfile", "setup/setup.sh" };

      var actual = new SortedSet<string>();

      var dockerFileArchive = new DockerfileArchive("Assets", "Dockerfile", image, NullLogger.Instance);

      // When
      using (var tarOut = new FileInfo(dockerFileArchive.Tar()).OpenRead())
      {
        using (var tarIn = TarArchive.CreateInputTarArchive(tarOut, Encoding.UTF8))
        {
          tarIn.ProgressMessageEvent += (_, entry, _) => actual.Add(entry.Name);
          tarIn.ListContents();
        }
      }

      // Then
      Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task ThrowsDockerfileDoesNotExist()
    {
      // Given
      var dockerfileDirectory = Directory.GetCurrentDirectory();

      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithDockerfile("Dockerfile")
        .WithDockerfileDirectory(dockerfileDirectory);

      // When
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.Build())
        .ConfigureAwait(false);

      // Then
      Assert.Equal($"Dockerfile does not exist in '{Path.GetFullPath(dockerfileDirectory)}'.", exception.Message);
    }

    [Fact]
    public async Task ThrowsDockerfileDirectoryDoesNotExist()
    {
      // Given
      var dockerfileDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("D"));

      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory(dockerfileDirectory);

      // When
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.Build())
        .ConfigureAwait(false);

      // Then
      Assert.Equal($"Directory '{Path.GetFullPath(dockerfileDirectory)}' does not exist.", exception.Message);
    }

    [Fact]
    public async Task BuildsDockerImage()
    {
      // Given
      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithName("alpine:custom")
        .WithDockerfile("Dockerfile")
        .WithDockerfileDirectory("Assets")
        .WithDeleteIfExists(true);

      // When
      var imageFromDockerfile1 = await imageFromDockerfileBuilder.Build()
        .ConfigureAwait(false);

      var imageFromDockerfile2 = await imageFromDockerfileBuilder.Build()
        .ConfigureAwait(false);

      // Then
      Assert.NotEmpty(imageFromDockerfile1);
      Assert.NotEmpty(imageFromDockerfile2);
      Assert.Equal(imageFromDockerfile1, imageFromDockerfile2);
    }
  }
}
