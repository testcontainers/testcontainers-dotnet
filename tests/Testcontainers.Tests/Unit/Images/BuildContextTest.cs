namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using Xunit;

  public sealed class BuildContextTest : IDisposable
  {
    private readonly string _contextDirectory = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"));

    public BuildContextTest()
    {
      Directory.CreateDirectory(_contextDirectory);
    }

    public void Dispose()
    {
      Directory.Delete(_contextDirectory, true);
    }

    [Fact]
    public async Task CreateImageShouldSucceedWhenContextContainsRequiredFiles()
    {
      // Given
      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithContextDirectory(_contextDirectory)
        .WithDockerfile("Dockerfile")
        .WithDockerfileDirectory("Assets/context/")
        .Build();

      // When
      await File.WriteAllBytesAsync(Path.Combine(_contextDirectory, "docker-entrypoint.sh"), Array.Empty<byte>(), TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      var exception = await Record.ExceptionAsync(() => imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Null(exception);
    }

    [Fact]
    public async Task CreateImageShouldFailWhenContextDoesNotContainRequiredFiles()
    {
      // Given
      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithContextDirectory(_contextDirectory)
        .WithDockerfile("Dockerfile")
        .WithDockerfileDirectory("Assets/context/")
        .Build();

      // When
      var exception = await Record.ExceptionAsync(() => imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.NotNull(exception);
    }
  }
}
