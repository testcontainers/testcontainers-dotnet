internal class BuildPaths
{
  private BuildPaths()
  {
  }

  public BuildFiles Files { get; private set; }
  public BuildDirectories Directories { get; private set; }

  public static Lazy<BuildPaths> Instance(ICakeContext context, string version)
  {
    return new Lazy<BuildPaths>(() =>
    {
      var baseDir = (DirectoryPath) context.Directory(".");

      var testResultsDir = baseDir.Combine("test-results");
      var testCoverageDir = baseDir.Combine("test-coverage");

      var artifactsDir = baseDir.Combine("artifacts");
      var artifactsVersionDir = artifactsDir.Combine(version);
      var artifactsBinDir = artifactsVersionDir.Combine("bin");
      var artifactsBinFullFx = artifactsBinDir.Combine("net461");
      var artifactsBinStandard = artifactsBinDir.Combine("netstandard2.0");
      var nugetRoot = artifactsVersionDir.Combine("nuget");

      return new BuildPaths
      {
        Files = new BuildFiles(),
        Directories = new BuildDirectories(
          testResultsDir,
          testCoverageDir,
          nugetRoot,
          artifactsDir,
          artifactsBinStandard
        )
      };
    });
  }
}

internal class BuildFiles
{
  public BuildFiles()
  {
  }
}

internal class BuildDirectories
{
  public DirectoryPath TestResults { get; }
  public DirectoryPath TestCoverage { get; }
  public DirectoryPath NugetRoot { get; }
  public DirectoryPath ArtifactsBinDir { get; }
  public DirectoryPath ArtifactsBinStandard { get; }
  public ICollection<DirectoryPath> ToClean { get; }

  public BuildDirectories(
    DirectoryPath testResultsDir,
    DirectoryPath testCoverageDir,
    DirectoryPath nugetRoot,
    DirectoryPath artifactsBinDir,
    DirectoryPath artifactsBinStandard
    )
  {
    TestResults = testResultsDir;
    TestCoverage = testCoverageDir;
    NugetRoot = nugetRoot;
    ArtifactsBinDir = artifactsBinDir;
    ArtifactsBinStandard = artifactsBinStandard;
    ToClean = new List<DirectoryPath>()
    {
      testResultsDir,
      testCoverageDir,
      artifactsBinDir,
      new DirectoryPath(".sonarqube")
    };
  }
}
