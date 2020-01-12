#load "./credentials.cake"
#load "./paths.cake"
#load "./projects.cake"
#load "./version.cake"

internal class BuildParameters
{
  private BuildParameters()
  {
  }

  public string Solution { get; private set; }
  public string Target { get; private set; }
  public string Configuration { get; private set; }
  public string Version { get; private set; }
  public string Branch { get; private set; }
  public string TestFilter { get; private set; }
  public bool IsLocalBuild { get; private set; }
  public bool ShouldPublish { get; private set; }
  public DotNetCoreVerbosity Verbosity { get; private set; }
  public SonarQubeCredentials SonarQubeCredentials { get; private set; }
  public NuGetCredentials NuGetCredentials { get; private set; }
  public BuildProjects Projects { get; private set; }
  public BuildPaths Paths { get; private set; }

  public static BuildParameters Instance(ICakeContext context, string solution)
  {
    var buildVersion = BuildVersion.Instance(context);

    var version = buildVersion.Version;

    var branch = buildVersion.Branch;

    var isLocalBuild = context.BuildSystem().IsLocalBuild;

    return new BuildParameters
    {
      Solution = context.MakeAbsolute(new DirectoryPath($"src/{solution}.sln")).FullPath,
      Target = context.Argument("target", "Default"),
      Configuration = context.Argument("configuration", "master".Equals(branch) ? "Release" : "Debug"),
      Version = version,
      Branch = branch,
      TestFilter = context.Argument<string>("test-filter", null),
      IsLocalBuild = isLocalBuild,
      ShouldPublish = !isLocalBuild && ShouldPublishing(branch),
      Verbosity = DotNetCoreVerbosity.Normal,
      SonarQubeCredentials = SonarQubeCredentials.GetSonarQubeCredentials(context),
      NuGetCredentials = NuGetCredentials.GetNuGetCredentials(context),
      Projects = BuildProjects.Instance(context, solution),
      Paths = BuildPaths.Instance(context, version)
    };
  }

  public static bool ShouldPublishing(string branch)
  {
    var branches = new [] { "master", "develop" };
    return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
  }
}
