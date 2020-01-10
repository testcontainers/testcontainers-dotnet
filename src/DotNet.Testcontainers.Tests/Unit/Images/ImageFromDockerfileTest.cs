namespace DotNet.Testcontainers.Tests.Unit.Images
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Images.Archives;
  using DotNet.Testcontainers.Images.Builders;
  using ICSharpCode.SharpZipLib.Tar;
  using Xunit;

  public class ImageFromDockerfileTest
  {
    [Fact]
    public void DockerfileArchiveTar()
    {
      // Given
      var expected = new List<string> { "Dockerfile", "setup", "setup/setup.sh" };

      var actual = new List<string>();

      var dockerFileArchive = new DockerfileArchive("./Assets");

      // When
      using (var tarOut = new FileInfo(dockerFileArchive.Tar()).OpenRead())
      {
        using (var tarIn = TarArchive.CreateInputTarArchive(tarOut))
        {
          tarIn.ProgressMessageEvent += (archive, entry, message) => actual.Add(entry.Name);
          tarIn.ListContents();
        }
      }

      // Then
      Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task DockerfileDoesNotExist()
    {
      // Given
      var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfile("Dockerfile")
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
