#tool nuget:?package=dotnet-sonarscanner&version=5.15.0

#addin nuget:?package=Cake.Sonar&version=1.1.32

#addin nuget:?package=Cake.Git&version=3.0.0

#load ".cake-scripts/parameters.cake"

readonly var param = BuildParameters.Instance(Context);

Setup(context =>
{
  var toClean = param.Paths.Directories.ToClean;

  foreach (var project in param.Projects.All)
  {
    toClean.Add("build");
  }

  Information("Building version {0} of Testcontainers ({1}@{2})", param.Version, param.Branch, param.Sha);
});

Teardown(context =>
{
});

Task("Clean")
  .Does(() =>
{
  var deleteDirectorySettings = new DeleteDirectorySettings();
  deleteDirectorySettings.Recursive = true;
  deleteDirectorySettings.Force = true;

  foreach (var directory in param.Paths.Directories.ToClean)
  {
    if (DirectoryExists(directory))
    {
      DeleteDirectory(directory, deleteDirectorySettings);
    }
  }
});

Task("Restore-NuGet-Packages")
  .Does(() =>
{
  DotNetRestore(param.Solution, new DotNetRestoreSettings
  {
    Verbosity = param.Verbosity
  });
});

Task("Build-Information")
  .Does(() =>
{
  foreach (var project in param.Projects.All)
  {
    Console.WriteLine($"{project.Name} [{project.Path.GetDirectory()}]");
  }
});

Task("Build")
  .Does(() =>
{
  DotNetBuild(param.Solution, new DotNetBuildSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    ArgumentCustomization = args => args
  });
});

Task("Tests")
  .Does(() =>
{
  DotNetTest(param.Solution, new DotNetTestSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    NoBuild = true,
    Collectors = new[] { "XPlat Code Coverage;Format=opencover" },
    Loggers = new[] { "trx" },
    Filter = param.TestFilter,
    ResultsDirectory = param.Paths.Directories.TestResultsDirectoryPath,
    ArgumentCustomization = args => args
  });
});

Task("Sonar-Begin")
  .Does(() =>
{
  SonarBegin(new SonarBeginSettings
  {
    Url = param.SonarQubeCredentials.Url,
    Key = param.SonarQubeCredentials.Key,
    Token = param.SonarQubeCredentials.Token,
    Organization = param.SonarQubeCredentials.Organization,
    Branch = param.IsPullRequest ? null : param.Branch, // A pull request analysis cannot have the branch analysis parameter 'sonar.branch.name'.
    UseCoreClr = true,
    Silent = true,
    Version = param.Version.Substring(0, 5),
    PullRequestProvider = "GitHub",
    PullRequestGithubEndpoint = "https://api.github.com/",
    PullRequestGithubRepository = "testcontainers/testcontainers-dotnet",
    PullRequestKey = param.IsPullRequest && System.Int32.TryParse(param.PullRequestId, out var id) ? id : (int?)null,
    PullRequestBranch = param.SourceBranch,
    PullRequestBase = param.TargetBranch,
    OpenCoverReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestResultsDirectoryPath)}/**/*.opencover.xml",
    VsTestReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestResultsDirectoryPath)}/**/*.trx"
  });
});

Task("Sonar-End")
  .Does(() =>
{
  SonarEnd(new SonarEndSettings
  {
    Token = param.SonarQubeCredentials.Token,
    UseCoreClr = true
  });
});

Task("Create-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  DotNetPack(param.Solution, new DotNetPackSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    NoBuild = true,
    IncludeSymbols = true,
    SymbolPackageFormat = "snupkg",
    OutputDirectory = param.Paths.Directories.NuGetDirectoryPath,
    ArgumentCustomization = args => args
      .Append("/p:ContinuousIntegrationBuild=true")
      .Append("/p:EmbedUntrackedSources=true")
      .Append($"/p:Version={param.Version}")
  });
});

Task("Sign-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish && false /* We do not have access to a valid code signing certificate anymore. */)
  .Does(() =>
{
  StartProcess("dotnet", new ProcessSettings
  {
    Arguments = new ProcessArgumentBuilder()
      .Append("nuget")
      .Append("sign")
      .AppendSwitchQuoted("--certificate-path", param.Paths.Files.CodeSigningCertificateFilePath.FullPath)
      .AppendSwitchQuoted("--certificate-password", param.CodeSigningCertificateCredentials.Password)
      .AppendSwitchQuoted("--timestamper", "http://ts.quovadisglobal.com/eu")
      .Append($"{MakeAbsolute(param.Paths.Directories.NuGetDirectoryPath)}/**/*.nupkg")
  });
});

Task("Publish-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  foreach(var package in GetFiles($"{param.Paths.Directories.NuGetDirectoryPath}/*.(nupkg|snupkgs)"))
  {
    DotNetNuGetPush(package.FullPath, new DotNetNuGetPushSettings
    {
      Source = param.NuGetCredentials.Source,
      ApiKey = param.NuGetCredentials.ApiKey,
      SkipDuplicate = true
    });
  }
});

Task("Default")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Build")
  .IsDependentOn("Tests");

Task("Sonar")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("Sonar-Begin")
  .IsDependentOn("Build")
  .IsDependentOn("Tests")
  .IsDependentOn("Sonar-End");

Task("Publish")
  .IsDependentOn("Create-NuGet-Packages")
  .IsDependentOn("Sign-NuGet-Packages")
  .IsDependentOn("Publish-NuGet-Packages");

RunTarget(param.Target);
