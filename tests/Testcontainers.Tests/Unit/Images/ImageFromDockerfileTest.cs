namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Images;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class ImageFromDockerfileTest
  {
    [Fact]
    public async Task DockerfileArchiveTar()
    {
      // Given
      var image = new DockerImage("testcontainers", "test", "0.1.0");

      var expected = new SortedSet<string> { ".dockerignore", "Dockerfile", "setup/setup.sh" };

      var actual = new SortedSet<string>();

      var dockerfileArchive = new DockerfileArchive("Assets", "Dockerfile", image, NullLogger.Instance);

      var dockerfileArchiveFilePath = await dockerfileArchive.Tar()
        .ConfigureAwait(false);

      // When
      using (var tarOut = new FileStream(dockerfileArchiveFilePath, FileMode.Open, FileAccess.Read))
      {
        using (var tarIn = TarArchive.CreateInputTarArchive(tarOut, Encoding.Default))
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
        .WithDockerfileDirectory(dockerfileDirectory)
        .Build();

      // When
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.CreateAsync())
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
        .WithDockerfileDirectory(dockerfileDirectory)
        .Build();

      // When
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.CreateAsync())
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
        .WithDeleteIfExists(true)
        .Build();

      // When
      await imageFromDockerfileBuilder.CreateAsync()
        .ConfigureAwait(false);

      await imageFromDockerfileBuilder.CreateAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Image, imageFromDockerfileBuilder.FullName));
      Assert.Null(Record.Exception(() => imageFromDockerfileBuilder.Repository));
      Assert.Null(Record.Exception(() => imageFromDockerfileBuilder.Name));
      Assert.Null(Record.Exception(() => imageFromDockerfileBuilder.Tag));
      Assert.Null(Record.Exception(() => imageFromDockerfileBuilder.FullName));
      Assert.Null(Record.Exception(() => imageFromDockerfileBuilder.GetHostname()));
    }
  }
}
