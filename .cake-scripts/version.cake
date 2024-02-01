#addin nuget:?package=Cake.Git&version=2.0.0

internal sealed class BuildInformation
{
  private BuildInformation()
  {
  }

  public string Sha { get; private set; }
  public string Branch { get; private set; }
  public string SourceBranch { get; private set; }
  public string TargetBranch { get; private set; }
  public string PullRequestId { get; private set; }
  public string Version { get; private set; }
  public bool IsLocalBuild { get; private set; }
  public bool IsReleaseBuild { get; private set; }
  public bool IsPullRequest { get; private set; }
  public bool ShouldPublish { get; private set; }

  public static BuildInformation Instance(ICakeContext context, string propertiesFilePath)
  {
    var buildSystem = context.BuildSystem();

    var isLocalBuild = buildSystem.IsLocalBuild;

    var environment = buildSystem.GitHubActions.Environment;

    var publishNuGetPackage = context.EnvironmentVariable("PUBLISH_NUGET_PACKAGE");

    var version = context.XmlPeek(propertiesFilePath, "/Project/PropertyGroup[1]/Version/text()");

    var git = context.GitBranchCurrent(".");

    var timeStamp = git.Tip.Committer.When.ToUnixTimeMilliseconds().ToString();

    var sha = git.Tip.Sha;

    var branch = git.FriendlyName;

    var isPullRequest = false;

    var isFork = false;

    var buildId = string.Empty;

    var pullRequestId = "0";

    string sourceBranch = null;

    string targetBranch = null;

    if (!isLocalBuild)
    {
      branch = environment.Workflow.RefName;
      isPullRequest = environment.PullRequest.IsPullRequest;
      isFork = "fork".Equals(environment.Workflow.EventName, StringComparison.OrdinalIgnoreCase);
      buildId = environment.Workflow.RunId;

      // Set build system environment variables and parameters.
      buildSystem.GitHubActions.Commands.SetEnvironmentVariable("semVer", version.Substring(0, 5));
    }

    if (isPullRequest)
    {
      pullRequestId = new string(environment.Workflow.Ref.Where(char.IsDigit).ToArray());
      sourceBranch = environment.Workflow.HeadRef;
      targetBranch = environment.Workflow.BaseRef;
    }

    var isReleaseBuild = GetIsReleaseBuild(branch);

    var shouldPublish = GetShouldPublish(branch) && ("1".Equals(publishNuGetPackage, StringComparison.Ordinal) || (bool.TryParse(publishNuGetPackage, out var result) && result));

    if (isFork && isPullRequest && shouldPublish)
    {
      throw new ArgumentException("Use 'feature/' or 'bugfix/' prefix for pull request branches.");
    }

    if (!isReleaseBuild)
    {
      version = $"{version}-beta";
    }

    if (!isReleaseBuild && !string.IsNullOrEmpty(buildId))
    {
      version = $"{version}.{buildId}";
    }

    return new BuildInformation
    {
      Sha = sha,
      Branch = branch,
      SourceBranch = sourceBranch,
      TargetBranch = targetBranch,
      PullRequestId = pullRequestId,
      Version = version,
      IsLocalBuild = isLocalBuild,
      IsReleaseBuild = isReleaseBuild,
      IsPullRequest = isPullRequest,
      ShouldPublish = shouldPublish
    };
  }

  private static bool GetIsReleaseBuild(string branch)
  {
    var branches = new[] { "main" };
    return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
  }

  private static bool GetShouldPublish(string branch)
  {
    var branches = new[] { "main", "develop" };
    return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
  }
}
