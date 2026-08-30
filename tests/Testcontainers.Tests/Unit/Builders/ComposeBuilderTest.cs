namespace DotNet.Testcontainers.Tests.Unit
{
  using System;
  using System.IO;
  using DotNet.Testcontainers.Builders;
  using DotNet.Testcontainers.Commons;
  using DotNet.Testcontainers.Containers;
  using Xunit;

  public sealed class ComposeBuilderTest
  {
    private static readonly string ComposeFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"), "compose.yml");

    static ComposeBuilderTest()
    {
      _ = Directory.CreateDirectory(Path.GetDirectoryName(ComposeFilePath)!);
      File.WriteAllText(ComposeFilePath, "services:\n  web:\n    image: \"" + CommonImages.Nginx.FullName + "\"\n");
    }

    public static bool IsNotWindows
    {
      get
      {
        return !OperatingSystem.IsWindows();
      }
    }

    [Fact]
    public void BuildsWhenComposeFilesInDifferentDirectoriesShareTheirName()
    {
      // Given
      // The Docker Compose files keep the path they have on the test host,
      // which allows Docker Compose files to share their name.
      var composeFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"), Path.GetFileName(ComposeFilePath));
      _ = Directory.CreateDirectory(Path.GetDirectoryName(composeFilePath)!);
      File.Copy(ComposeFilePath, composeFilePath);

      // When
      var exception = Record.Exception(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath, composeFilePath).Build());

      // Then
      Assert.Null(exception);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenComposeFileIsNotSet()
    {
      // Given
      var composeBuilder = new ComposeBuilder(CommonImages.DockerCli);

      // When
      var exception = Assert.Throws<ArgumentException>(composeBuilder.Build);

      // Then
      Assert.StartsWith("At least one Docker Compose file must be set.", exception.Message);
    }

    [Fact]
    public void ThrowsFileNotFoundExceptionWhenComposeFileDoesNotExist()
    {
      Assert.Throws<FileNotFoundException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(Path.Combine(TestSession.TempDirectoryPath, "not-found.yml")).Build());
    }

    [Fact(SkipUnless = nameof(IsNotWindows), Skip = "A Windows path cannot contain the path separator characters.")]
    public void ThrowsArgumentExceptionWhenComposeFilePathContainsEveryPathSeparator()
    {
      // Given
      // Docker Compose separates the Docker Compose file paths in COMPOSE_FILE with a
      // path separator character. A path that contains every supported separator
      // cannot be passed to Docker Compose.
      var composeFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D") + ":;|,", Path.GetFileName(ComposeFilePath));
      _ = Directory.CreateDirectory(Path.GetDirectoryName(composeFilePath)!);
      File.Copy(ComposeFilePath, composeFilePath);

      var composeBuilder = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(composeFilePath);

      // When
      var exception = Assert.Throws<ArgumentException>(composeBuilder.Build);

      // Then
      Assert.StartsWith("The Docker Compose file paths contain every supported path separator character", exception.Message);
    }

    [Fact]
    public void ThrowsFileNotFoundExceptionWhenFileCopyInclusionDoesNotExist()
    {
      Assert.Throws<FileNotFoundException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithCopyFilesInContainer("not-found.txt").Build());
    }

    [Fact]
    public void AppendsRandomSuffixToProjectNamePrefix()
    {
      // Given
      const string projectNamePrefix = "unit-test";

      // When
      var composeContainer = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix).Build();

      // Then
      Assert.StartsWith(projectNamePrefix + "-", composeContainer.ProjectName);
      Assert.NotEqual(projectNamePrefix, composeContainer.ProjectName);
    }

    [Theory]
    [InlineData("-invalid")]
    [InlineData("_invalid")]
    [InlineData("Invalid")]
    [InlineData("invalid name")]
    public void ThrowsArgumentExceptionWhenProjectNamePrefixIsInvalid(string projectNamePrefix)
    {
      // Given
      var composeBuilder = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix);

      // When
      var exception = Assert.Throws<ArgumentException>(composeBuilder.Build);

      // Then
      Assert.StartsWith("The Docker Compose project name prefix", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenProjectNamePrefixIsTooLong()
    {
      // Given
      var projectNamePrefix = new string('a', 55);
      var composeBuilder = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix);

      // When
      var exception = Assert.Throws<ArgumentException>(composeBuilder.Build);

      // Then
      Assert.StartsWith("The Docker Compose project name prefix", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenReuseIsEnabled()
    {
      // Given
      var composeBuilder = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithReuse(true);

      // When
      var exception = Assert.Throws<ArgumentException>(composeBuilder.Build);

      // Then
      Assert.StartsWith("Reuse cannot be used", exception.Message);
    }

    [Fact]
    public void ThrowsComposeServiceNotExposedExceptionWhenServiceIsNotExposed()
    {
      // Given
      var composeContainer = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).Build();

      // When
      var exception = Assert.Throws<ComposeServiceNotExposedException>(() => composeContainer.GetServicePort("web", 80));

      // Then
      Assert.Contains("'web-1'", exception.Message);
    }
  }
}
