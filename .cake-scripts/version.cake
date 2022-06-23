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

    var environment = buildSystem.GitHubActions.Environment;

    var isLocalBuild = buildSystem.IsLocalBuild;

    var isPullRequest = environment.PullRequest.IsPullRequest;

    var isFork = "fork".Equals(environment.Workflow.EventName, StringComparison.OrdinalIgnoreCase);

    var buildId = environment.Workflow.RunId;

    var git = context.GitBranchCurrent(".");

    var timeStamp = git.Tip.Committer.When.ToUnixTimeMilliseconds().ToString();

    var sha = git.Tip.Sha;

    string branch;

    string pullRequestId = "0";

    string sourceBranch = null;

    string targetBranch = null;

    if (isLocalBuild)
    {
      branch = git.FriendlyName;
    }
    else
    {
      branch = environment.Workflow.RefName;
    }

    if (isPullRequest)
    {
      pullRequestId = new string(environment.Workflow.Ref.Where(char.IsDigit).ToArray());
      sourceBranch = environment.Workflow.HeadRef;
      targetBranch = environment.Workflow.BaseRef;
    }

    var version = context.XmlPeek(propertiesFilePath, "/Project/PropertyGroup[2]/Version/text()");

    var isReleaseBuild = GetIsReleaseBuild(branch);

    var shouldPublish = GetShouldPublish(branch) && "true".Equals(context.EnvironmentVariable("PUBLISH_NUGET_PACKAGE"), StringComparison.OrdinalIgnoreCase);

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
    var branches = new[] { "master" };
    return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
  }

  private static bool GetShouldPublish(string branch)
  {
    var branches = new[] { "master", "develop" };
    return branches.Any(b => StringComparer.OrdinalIgnoreCase.Equals(b, branch));
  }
}
