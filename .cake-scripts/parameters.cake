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
  public bool IsLocalBuild { get; private set; }
  public bool IsReleaseBuild { get; private set; }
  public bool IsPullRequest { get; private set; }
  public bool ShouldPublish { get; private set; }
  public DotNetCoreVerbosity Verbosity { get; private set; }
  public SonarQubeCredentials SonarQubeCredentials { get; private set; }
  public NuGetCredentials NuGetCredentials { get; private set; }
  public BuildProjects Projects { get; private set; }
  public BuildPaths Paths { get; private set; }

  public static BuildParameters Instance(ICakeContext context, string solution)
  {
    var buildInformation = BuildInformation.Instance(context);

    return new BuildParameters
    {
      Solution = context.MakeAbsolute(new DirectoryPath($"src/{solution}.sln")).FullPath,
      Target = context.Argument("target", "Default"),
      Configuration = context.Argument("configuration", buildInformation.IsReleaseBuild ? "Release" : "Debug"),
      Sha = buildInformation.Sha,
      Branch = buildInformation.Branch,
      SourceBranch = buildInformation.SourceBranch,
      TargetBranch = buildInformation.TargetBranch,
      PullRequestId = buildInformation.PullRequestId,
      Version = buildInformation.Version,
      TestFilter = context.Argument<string>("test-filter", null),
      IsLocalBuild = buildInformation.IsLocalBuild,
      IsReleaseBuild = !buildInformation.IsLocalBuild && buildInformation.IsReleaseBuild,
      IsPullRequest = buildInformation.IsPullRequest,
      ShouldPublish = !buildInformation.IsLocalBuild && buildInformation.ShouldPublish,
      Verbosity = DotNetCoreVerbosity.Quiet,
      SonarQubeCredentials = SonarQubeCredentials.GetSonarQubeCredentials(context),
      NuGetCredentials = NuGetCredentials.GetNuGetCredentials(context),
      Projects = BuildProjects.Instance(context, solution),
      Paths = BuildPaths.Instance(context, buildInformation.Version)
    };
  }
}
