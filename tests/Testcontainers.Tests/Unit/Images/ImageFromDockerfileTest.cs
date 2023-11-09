namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Linq;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Images;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class ImageFromDockerfileTest
  {
    [Fact]
    public void DockerfileArchiveGetBaseImages()
    {
      // Given
      IImage image = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      var dockerfileArchive = new DockerfileArchive("Assets/pullBaseImages/", "Dockerfile", image, NullLogger.Instance);

      // When
      var baseImages = dockerfileArchive.GetBaseImages().ToArray();

      // Then
      Assert.Equal(4, baseImages.Length);
      Assert.Contains(baseImages, item => "mcr.microsoft.com/dotnet/sdk:6.0".Equals(item.FullName));
      Assert.Contains(baseImages, item => "mcr.microsoft.com/dotnet/runtime:6.0".Equals(item.FullName));
      Assert.Contains(baseImages, item => "mcr.microsoft.com/dotnet/aspnet:6.0.22-jammy-amd64".Equals(item.FullName));
      Assert.Contains(baseImages, item => "mcr.microsoft.com/dotnet/aspnet:6.0.23-jammy-amd64".Equals(item.FullName));
    }

    [Fact]
    public async Task DockerfileArchiveTar()
    {
      // Given
      IImage image = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      var expected = new SortedSet<string> { ".dockerignore", "Dockerfile", "setup/setup.sh" };

      var actual = new SortedSet<string>();

      var dockerfileArchive = new DockerfileArchive("Assets/", "Dockerfile", image, NullLogger.Instance);

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
      IImage tag1 = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      IImage tag2 = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithName(tag1)
        .WithDockerfile("Dockerfile")
        .WithDockerfileDirectory("Assets/")
        .WithDeleteIfExists(true)
        .WithCreateParameterModifier(parameterModifier => parameterModifier.Tags.Add(tag2.FullName))
        .Build();

      // When
      await imageFromDockerfileBuilder.CreateAsync()
        .ConfigureAwait(false);

      await imageFromDockerfileBuilder.CreateAsync()
        .ConfigureAwait(false);

      // Then
      Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Image, tag1.FullName));
      Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Image, tag2.FullName));
      Assert.NotNull(imageFromDockerfileBuilder.Repository);
      Assert.NotNull(imageFromDockerfileBuilder.Name);
      Assert.NotNull(imageFromDockerfileBuilder.Tag);
      Assert.NotNull(imageFromDockerfileBuilder.FullName);
      Assert.Null(imageFromDockerfileBuilder.GetHostname());
    }
  }
}
