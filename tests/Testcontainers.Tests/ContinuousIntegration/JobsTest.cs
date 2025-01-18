namespace DotNet.Testcontainers.Tests.ContinuousIntegration
{
  using System.IO;
  using System.Linq;
  using System.Runtime.CompilerServices;
  using System.Text.RegularExpressions;
  using Xunit;

  public partial class JobsTest
  {
    [GeneratedRegex("name: \"(.+?)\"")]
    private static partial Regex ProjectNameRegex();

    [Fact]
    public void AllTestProjectsShouldBeConfiguredForContinuousIntegration()
    {
      var ciCdFilePath = GetCiCdFilePath();
      var configuredProjects = File.ReadAllLines(ciCdFilePath).Select(line => ProjectNameRegex().Match(line).Groups[1].Value).Where(line => line.Length > 0).ToList();
      Assert.NotEmpty(configuredProjects);

      var existingProjects = Directory.GetFiles(GetTestsPath(), "*.Tests.csproj", SearchOption.AllDirectories).Select(name => Path.GetFileName(name)[..^13]).ToList();
      Assert.NotEmpty(existingProjects);

      var missingConfiguredProjects = existingProjects.Except(configuredProjects).ToList();
      if (missingConfiguredProjects.Count > 0)
      {
        Assert.Fail($"{string.Join(", ", missingConfiguredProjects)} must be configured in {ciCdFilePath}");
      }
    }

    private static string GetCiCdFilePath() => Path.Combine(GetRepositoryPath(), ".github", "workflows", "cicd.yml");

    private static string GetTestsPath() => Path.Combine(GetRepositoryPath(), "tests");

    private static string GetRepositoryPath([CallerFilePath] string path = "") => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path)!, "..", "..", ".."));
  }
}
