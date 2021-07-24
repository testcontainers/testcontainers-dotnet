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

    var isPullRequest = buildSystem.AzurePipelines.Environment.PullRequest.IsPullRequest;

    var isFork = buildSystem.AzurePipelines.Environment.PullRequest.IsFork;

    var buildId = buildSystem.AzurePipelines.Environment.Build.Id.ToString();

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
      branch = buildSystem.AzurePipelines.Environment.Repository.SourceBranch;
      branch = branch.Replace("refs/heads/", string.Empty);
    }

    if (isPullRequest)
    {
      pullRequestId = buildSystem.AzurePipelines.Environment.PullRequest.Number.ToString();
      sourceBranch = buildSystem.AzurePipelines.Environment.PullRequest.SourceBranch;
      targetBranch = buildSystem.AzurePipelines.Environment.PullRequest.TargetBranch;
      sourceBranch = sourceBranch.Replace("refs/heads/", string.Empty);
      targetBranch = targetBranch.Replace("refs/heads/", string.Empty);
    }

    var version = context.XmlPeek(propertiesFilePath, "/Project/PropertyGroup[1]/Version/text()");

    var isReleaseBuild = GetIsReleaseBuild(branch);

    var shouldPublish = GetShouldPublish(branch);

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
