namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Core.Builder;
  using Xunit;

  public class ImageFromDockerfileTest
  {
    [Fact]
    public async Task DockerfileDoesNotExist()
    {
      await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory(string.Empty)
          .Build());
    }

    [Fact]
    public async Task DockerfileDirectoryDoesNotExist()
    {
      await Assert.ThrowsAsync<ArgumentException>(() =>
        new ImageFromDockerfileBuilder()
          .WithDockerfileDirectory("DoesNotExist")
          .Build());
    }

    [Fact]
    public async Task SimpleDockerfile()
    {
      var imageFromDockerfile = await new ImageFromDockerfileBuilder()
        .WithName("alpine:custom")
        .WithDockerfileDirectory("Assets")
        .WithDeleteIfExists(false)
        .Build();

      Assert.NotEmpty(imageFromDockerfile);
    }
  }
}
