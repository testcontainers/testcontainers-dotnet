internal sealed class BuildPaths
{
  private BuildPaths()
  {
  }

  public BuildFiles Files { get; private set; }
  public BuildDirectories Directories { get; private set; }

  public static BuildPaths Instance(ICakeContext context, string version)
  {
    var baseDir = (DirectoryPath) context.Directory(".");

    var testResultsDir = baseDir.Combine("test-results");
    var testCoverageDir = baseDir.Combine("test-coverage");

    var artifactsDir = baseDir.Combine("artifacts");
    var artifactsVersionDir = artifactsDir.Combine(version);
    var nugetRoot = artifactsVersionDir.Combine("nuget");

    return new BuildPaths
    {
      Files = new BuildFiles(),
      Directories = new BuildDirectories(
        testResultsDir,
        testCoverageDir,
        nugetRoot,
        artifactsDir
      )
    };
  }
}

internal sealed class BuildFiles
{
  public BuildFiles()
  {
  }
}

internal sealed class BuildDirectories
{
  public DirectoryPath TestResults { get; }
  public DirectoryPath TestCoverage { get; }
  public DirectoryPath NugetRoot { get; }
  public DirectoryPath ArtifactsBinDir { get; }
  public ICollection<DirectoryPath> ToClean { get; }

  public BuildDirectories(
    DirectoryPath testResultsDir,
    DirectoryPath testCoverageDir,
    DirectoryPath nugetRoot,
    DirectoryPath artifactsBinDir)
  {
    TestResults = testResultsDir;
    TestCoverage = testCoverageDir;
    NugetRoot = nugetRoot;
    ArtifactsBinDir = artifactsBinDir;
    ToClean = new List<DirectoryPath>()
    {
      testResultsDir,
      testCoverageDir,
      artifactsBinDir,
      new DirectoryPath(".sonarqube")
    };
  }
}
