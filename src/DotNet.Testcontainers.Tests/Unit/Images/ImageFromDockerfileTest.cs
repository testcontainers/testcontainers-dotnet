namespace DotNet.Testcontainers.Tests.Unit.Images
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Images;
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
      var expected = new SortedSet<string> { "Dockerfile", "setup/setup.sh" };

      var actual = new SortedSet<string>();

      var dockerFileArchive = new DockerfileArchive("Assets", "Dockerfile", new DockerImage("Testcontainers", "Test", "1.0.0"));

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
      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithName("alpine:custom")
        .WithDockerfileDirectory("Assets")
        .WithDeleteIfExists(true);

      // When
      var imageFromDockerfile1 = await imageFromDockerfileBuilder.Build();
      var imageFromDockerfile2 = await imageFromDockerfileBuilder.Build(); // Deletes the previously created image.

      // Then
      Assert.NotEmpty(imageFromDockerfile1);
      Assert.NotEmpty(imageFromDockerfile2);
      Assert.Equal(imageFromDockerfile1, imageFromDockerfile2);
    }
  }
}
