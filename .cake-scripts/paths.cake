internal sealed class BuildPaths
{
  private BuildPaths()
  {
  }

  public BuildFiles Files { get; private set; }
  public BuildDirectories Directories { get; private set; }

  public static BuildPaths Instance(ICakeContext context, string version)
  {
    var baseDir = (DirectoryPath)context.Directory(".");

    var testResultsDir = baseDir.Combine("test-results");
    var testCoverageDir = baseDir.Combine("test-coverage");

    var artifactsDir = baseDir.Combine("artifacts");
    var artifactsVersionDir = artifactsDir.Combine(version);
    var nugetDir = artifactsVersionDir.Combine("nuget");

    return new BuildPaths
    {
      Files = new BuildFiles(),
      Directories = new BuildDirectories(
        testResultsDir,
        testCoverageDir,
        nugetDir,
        artifactsDir
      )
    };
  }
}

internal sealed class BuildFiles
{
  public FilePath CodeSigningCertificateFilePath { get; }

  public BuildFiles()
  {
    CodeSigningCertificateFilePath = new FilePath("code-signing-certificate.pfx");
  }
}

internal sealed class BuildDirectories
{
  public DirectoryPath TestResultsDirectoryPath { get; }
  public DirectoryPath TestCoverageDirectoryPath { get; }
  public DirectoryPath NuGetDirectoryPath { get; }
  public DirectoryPath ArtifactsDirectoryPath { get; }
  public ICollection<DirectoryPath> ToClean { get; }

  public BuildDirectories(
    DirectoryPath testResultsDir,
    DirectoryPath testCoverageDir,
    DirectoryPath nugetDir,
    DirectoryPath artifactsDir)
  {
    TestResultsDirectoryPath = testResultsDir;
    TestCoverageDirectoryPath = testCoverageDir;
    NuGetDirectoryPath = nugetDir;
    ArtifactsDirectoryPath = artifactsDir;
    ToClean = new List<DirectoryPath>()
    {
      testResultsDir,
      testCoverageDir,
      nugetDir,
      artifactsDir,
      new DirectoryPath(".sonarqube")
    };
  }
}
