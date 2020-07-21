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

  public static BuildInformation Instance(ICakeContext context)
  {
    var isFork = context.EnvironmentVariable("SYSTEM_PULLREQUEST_ISFORK", "False");

    var buildReason = context.EnvironmentVariable("BUILD_REASON", string.Empty);

    var buildNumber = context.EnvironmentVariable("BUILD_BUILDNUMBER", string.Empty);

    var git = context.GitBranchCurrent(".");

    var timeStamp = git.Tip.Committer.When.ToUnixTimeMilliseconds().ToString();

    var sha = git.Tip.Sha;

    var branch = string.Empty;

    string sourceBranch = null;

    string targetBranch = null;

    string pullRequestId = null;

    var isPullRequest = "PullRequest".Equals(buildReason);

    if (isPullRequest)
    {
      branch = context.EnvironmentVariable("SYSTEM_PULLREQUEST_SOURCEBRANCH", git.FriendlyName);
      sourceBranch = context.EnvironmentVariable("SYSTEM_PULLREQUEST_SOURCEBRANCH");
      targetBranch = context.EnvironmentVariable("SYSTEM_PULLREQUEST_TARGETBRANCH");
      pullRequestId = context.EnvironmentVariable("SYSTEM_PULLREQUEST_PULLREQUESTNUMBER");
      sourceBranch = sourceBranch?.Replace("refs/heads/", string.Empty);
      targetBranch = targetBranch?.Replace("refs/heads/", string.Empty);
    }
    else
    {
      branch = context.EnvironmentVariable("BUILD_SOURCEBRANCH", git.FriendlyName);
    }

    branch = branch.Replace("refs/heads/", string.Empty);

    var version = context.XmlPeek("src/Shared.msbuild", "/Project/PropertyGroup[1]/Version/text()");

    var isLocalBuild = context.BuildSystem().IsLocalBuild;

    var isReleaseBuild = GetIsReleaseBuild(branch);

    var shouldPublish = GetShouldPublish(branch);

    if (bool.Parse(isFork) && isPullRequest && shouldPublish)
    {
      throw new ArgumentException("Use 'feature/' or 'bugfix/' prefix for pull request branches.");
    }

    if (!isReleaseBuild)
    {
      version = $"{version}-beta";
    }

    if (!isReleaseBuild && !string.IsNullOrEmpty(buildNumber))
    {
      version = $"{version}.{buildNumber}";
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
