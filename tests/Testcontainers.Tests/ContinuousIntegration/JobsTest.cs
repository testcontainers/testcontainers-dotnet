namespace DotNet.Testcontainers.Tests.ContinuousIntegration
{
  using System.IO;
  using System.Linq;
  using System.Text.RegularExpressions;
  using DotNet.Testcontainers.Builders;
  using Xunit;

  public sealed partial class JobsTest
  {
    [GeneratedRegex("name: \"(.+?)\"")]
    private static partial Regex ProjectNameRegex();

    [Fact]
    public void AllTestProjectsShouldBeConfiguredForContinuousIntegration()
    {
      var ciCdFilePath = GetCiCdFilePath();

      var expectedProjects = File.ReadAllLines(ciCdFilePath).Select(line => ProjectNameRegex().Match(line).Groups[1].Value).Where(line => line.Length > 0).ToList();
      Assert.NotEmpty(expectedProjects);

      var actualProjects = Directory.GetFiles(GetTestsDirectoryPath(), "*.Tests.csproj", SearchOption.AllDirectories).Select(name => Path.GetFileName(name)[..^13]).ToList();
      Assert.NotEmpty(actualProjects);

      var missingConfiguredProjects = actualProjects.Except(expectedProjects).ToList();
      Assert.True(missingConfiguredProjects.Count == 0, $"{string.Join(", ", missingConfiguredProjects)} must be configured in '{ciCdFilePath}'.");
    }

    private static string GetCiCdFilePath() => Path.Combine(GetRepositoryDirectoryPath(), ".github", "workflows", "cicd.yml");

    private static string GetTestsDirectoryPath() => Path.Combine(GetRepositoryDirectoryPath(), "tests");

    private static string GetRepositoryDirectoryPath() => CommonDirectoryPath.GetSolutionDirectory().DirectoryPath;
  }
}
