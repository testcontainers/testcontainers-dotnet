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
      // The Docker Compose files keep the path they have on the test host, which
      // allows Docker Compose files to share their name.
      var composeFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D"), Path.GetFileName(ComposeFilePath));
      _ = Directory.CreateDirectory(Path.GetDirectoryName(composeFilePath)!);
      File.Copy(ComposeFilePath, composeFilePath);

      _ = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath, composeFilePath).Build();
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenComposeFileIsNotSet()
    {
      var exception = Assert.Throws<ArgumentException>(() => new ComposeBuilder(CommonImages.DockerCli).Build());
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
      // Docker Compose separates the Docker Compose file paths in COMPOSE_FILE with a
      // path separator character. A path that contains every supported separator
      // cannot be passed to Docker Compose.
      var composeFilePath = Path.Combine(TestSession.TempDirectoryPath, Guid.NewGuid().ToString("D") + ":;|,", Path.GetFileName(ComposeFilePath));
      _ = Directory.CreateDirectory(Path.GetDirectoryName(composeFilePath)!);
      File.Copy(ComposeFilePath, composeFilePath);

      var exception = Assert.Throws<ArgumentException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(composeFilePath).Build());
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
      const string projectNamePrefix = "unit-test";
      var composeContainer = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix).Build();
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
      var exception = Assert.Throws<ArgumentException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix).Build());
      Assert.StartsWith("The Docker Compose project name prefix", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenProjectNamePrefixIsTooLong()
    {
      var projectNamePrefix = new string('a', 55);
      var exception = Assert.Throws<ArgumentException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithProjectNamePrefix(projectNamePrefix).Build());
      Assert.StartsWith("The Docker Compose project name prefix", exception.Message);
    }

    [Fact]
    public void ThrowsArgumentExceptionWhenReuseIsEnabled()
    {
      var exception = Assert.Throws<ArgumentException>(() => new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).WithReuse(true).Build());
      Assert.StartsWith("Reuse cannot be used", exception.Message);
    }

    [Fact]
    public void ThrowsComposeServiceNotExposedExceptionWhenServiceIsNotExposed()
    {
      var composeContainer = new ComposeBuilder(CommonImages.DockerCli).WithComposeFile(ComposeFilePath).Build();
      Assert.Throws<ComposeServiceNotExposedException>(() => composeContainer.GetServicePort("web", 80));
    }
  }
}
