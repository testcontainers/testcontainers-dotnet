namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.Collections.Generic;
  using System.Collections.ObjectModel;
  using System.IO;
  using System.Text;
  using System.Threading.Tasks;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Images;
  using ICSharpCode.SharpZipLib.Tar;
  using Microsoft.Extensions.Logging;
  using Microsoft.Extensions.Logging.Abstractions;
  using Xunit;

  public sealed class ImageFromDockerfileTest
  {
    [Fact]
    public void DockerfileArchiveGetBaseImages()
    {
      // Given
      var expected = new[]
      {
        new DockerImage("mcr.microsoft.com/dotnet/sdk:8.0"),
        new DockerImage("mcr.microsoft.com/dotnet/runtime:8.0"),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-jammy"),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-noble"),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-alpine"),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0", new Platform("linux/amd64")),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0", new Platform("linux/arm64")),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0", new Platform("linux/arm/v6")),
        new DockerImage("mcr.microsoft.com/dotnet/aspnet:8.0-azurelinux3.0", new Platform("linux/arm/v7")),
        new DockerImage("mcr.microsoft.com/dotnet/sdk:8.0.414"),
      };

      IImage image = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      // The Dockerfile does not contain a default value.
      var buildArguments = new Dictionary<string, string>();
      buildArguments.Add("SDK_VERSION_8_0", "8.0.414");

      var dockerfileArchive = new DockerfileArchive(null, "Assets/pullBaseImages/", "Dockerfile", image, buildArguments, NullLogger.Instance);

      // When
      var actual = dockerfileArchive.GetBaseImages();

      // Then
      Assert.Equivalent(expected, actual);
    }

    [Fact]
    public async Task DockerfileArchiveTar()
    {
      // Given
      IImage image = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      var expected = new SortedSet<string> { ".dockerignore", "Dockerfile", "setup/setup.sh" };

      var actual = new SortedSet<string>();

      var buildArguments = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

      var dockerfileArchive = new DockerfileArchive(null, "Assets/", "Dockerfile", image, buildArguments, NullLogger.Instance);

      var dockerfileArchiveFilePath = await dockerfileArchive.Tar(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

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
    public async Task IgnoredDockerfileIsCopiedToTarball()
    {
      // Given

      // This test ensures Dockerfiles in subdirectories are copied to the right paths
      // in the tarball, instead of ending up at the root directory:
      // https://github.com/testcontainers/testcontainers-dotnet/issues/1557.
      IImage image = new DockerImage("localhost/testcontainers", Guid.NewGuid().ToString("D"), string.Empty);

      var actual = new SortedSet<string>();

      var buildArguments = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>());

      var dockerfileArchive = new DockerfileArchive(null, "Assets/", "target/Dockerfile", image, buildArguments, NullLogger.Instance);

      var dockerfileArchiveFilePath = await dockerfileArchive.Tar(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

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
      Assert.Contains("target/Dockerfile", actual);
      Assert.DoesNotContain("Dockerfile", actual);
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
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

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
      var exception = await Assert.ThrowsAsync<ArgumentException>(() => imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Equal($"Directory '{Path.GetFullPath(dockerfileDirectory)}' does not exist.", exception.Message);
    }

    [Fact]
    public async Task BuildsDockerScratchImage()
    {
      // Given
      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory("Assets/scratch/")
        .Build();

      // When
      var exception = await Record.ExceptionAsync(() => imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken))
        .ConfigureAwait(true);

      // Then
      Assert.Null(exception);
      Assert.NotNull(imageFromDockerfileBuilder.Repository);
      Assert.NotNull(imageFromDockerfileBuilder.Tag);
      Assert.NotNull(imageFromDockerfileBuilder.FullName);
      Assert.Null(imageFromDockerfileBuilder.GetHostname());
    }

    [Theory]
    [InlineData("Dockerfile")]
    [InlineData("./Dockerfile")]
    [InlineData(".\\Dockerfile")]
    public async Task BuildsDockerAlpineImage(string dockerfile)
    {
      // Given
      IImage tag1 = new DockerImage(new DockerImage(string.Join("/", "localhost", "testcontainers", Guid.NewGuid().ToString("D"))));

      IImage tag2 = new DockerImage(new DockerImage(string.Join("/", "localhost", "testcontainers", Guid.NewGuid().ToString("D"))));

      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithName(tag1)
        .WithDockerfile(dockerfile)
        .WithDockerfileDirectory("Assets/")
        .WithDeleteIfExists(true)
        .WithCreateParameterModifier(parameterModifier => parameterModifier.Tags.Add(tag2.FullName))
        .Build();

      // When
      await imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      await imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Image, tag1.FullName));
      Assert.True(DockerCli.ResourceExists(DockerCli.DockerResource.Image, tag2.FullName));
      Assert.NotNull(imageFromDockerfileBuilder.Repository);
      Assert.NotNull(imageFromDockerfileBuilder.Tag);
      Assert.NotNull(imageFromDockerfileBuilder.FullName);
      Assert.Null(imageFromDockerfileBuilder.GetHostname());
    }

    [Fact]
    public async Task BuildTargetBuildsUpToExpectedTarget()
    {
      // Given
      var logger = new TestLogger();

      var imageFromDockerfileBuilder = new ImageFromDockerfileBuilder()
        .WithDockerfileDirectory("Assets/target/")
        .WithTarget("build")
        .WithLogger(logger)
        .Build();

      // When
      await imageFromDockerfileBuilder.CreateAsync(TestContext.Current.CancellationToken)
        .ConfigureAwait(true);

      // Then
      Assert.Contains(logger.Logs, line => line.Contains("FROM scratch AS base"));
      Assert.Contains(logger.Logs, line => line.Contains("FROM base AS build"));
      Assert.DoesNotContain(logger.Logs, line => line.Contains("FROM build AS final"));
    }

    private sealed class TestLogger : ILogger
    {
      public IList<string> Logs { get; }
        = new List<string>();

      public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        => Logs.Add(formatter(state, exception));

      public bool IsEnabled(LogLevel logLevel)
        => true;

      public IDisposable BeginScope<TState>(TState state) where TState : notnull
        => null;
    }
  }
}
