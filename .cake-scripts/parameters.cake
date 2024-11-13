#load "./credentials.cake"
#load "./paths.cake"
#load "./projects.cake"
#load "./version.cake"

internal sealed class BuildParameters
{
  private BuildParameters()
  {
  }

  public string Solution { get; private set; }
  public string Target { get; private set; }
  public string Configuration { get; private set; }
  public string Sha { get; private set; }
  public string Branch { get; private set; }
  public string SourceBranch { get; private set; }
  public string TargetBranch { get; private set; }
  public string PullRequestId { get; private set; }
  public string Version { get; private set; }
  public string TestFilter { get; private set; }
  public string TestProject { get; private set; }
  public bool IsLocalBuild { get; private set; }
  public bool IsReleaseBuild { get; private set; }
  public bool IsPullRequest { get; private set; }
  public bool ShouldPublish { get; private set; }
  public DotNetVerbosity Verbosity { get; private set; }
  public BuildCredentials CodeSigningCertificateCredentials { get; private set; }
  public SonarQubeCredentials SonarQubeCredentials { get; private set; }
  public NuGetCredentials NuGetCredentials { get; private set; }
  public BuildProjects Projects { get; private set; }
  public BuildPaths Paths { get; private set; }

  public static BuildParameters Instance(ICakeContext context)
  {
    const string propertiesFilePath = "Directory.Build.props";
    const string solutionFilePath = "Testcontainers.sln";

    var buildInformation = BuildInformation.Instance(context, propertiesFilePath);

    return new BuildParameters
    {
      Solution = context.MakeAbsolute(new DirectoryPath(solutionFilePath)).FullPath,
      Target = context.Argument("target", "Default"),
      Configuration = context.Argument("configuration", buildInformation.IsReleaseBuild ? "Release" : "Debug"),
      Sha = buildInformation.Sha,
      Branch = buildInformation.Branch,
      SourceBranch = buildInformation.SourceBranch,
      TargetBranch = buildInformation.TargetBranch,
      PullRequestId = buildInformation.PullRequestId,
      Version = buildInformation.Version,
      TestFilter = context.Argument<string>("test-filter", null),
      TestProject = context.Argument<string>("test-project", null),
      IsLocalBuild = buildInformation.IsLocalBuild,
      IsReleaseBuild = !buildInformation.IsLocalBuild && buildInformation.IsReleaseBuild,
      IsPullRequest = buildInformation.IsPullRequest,
      ShouldPublish = !buildInformation.IsLocalBuild && buildInformation.ShouldPublish,
      Verbosity = DotNetVerbosity.Quiet,
      CodeSigningCertificateCredentials = BuildCredentials.GetCodeSigningCertificateCredentials(context),
      SonarQubeCredentials = SonarQubeCredentials.GetSonarQubeCredentials(context),
      NuGetCredentials = NuGetCredentials.GetNuGetCredentials(context),
      Projects = BuildProjects.Instance(context, solutionFilePath),
      Paths = BuildPaths.Instance(context, buildInformation.Version)
    };
  }
}
