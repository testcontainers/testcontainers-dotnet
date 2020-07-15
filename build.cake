#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.8.0

#addin nuget:?package=Cake.Sonar&version=1.1.25

#addin nuget:?package=Cake.Git&version=0.22.0

#load ".cake-scripts/parameters.cake"

readonly var param = BuildParameters.Instance(Context, "DotNet.Testcontainers");

Setup(context =>
{
  var toClean = param.Paths.Directories.ToClean;

  foreach (var project in param.Projects.All)
  {
    toClean.Add(project.Path.GetDirectory().Combine("obj"));
    toClean.Add(project.Path.GetDirectory().Combine("bin"));
    toClean.Add(project.Path.GetDirectory().Combine("Release"));
    toClean.Add(project.Path.GetDirectory().Combine("Debug"));
  }

  Information("Building version {0} of .NET Testcontainers ({1}@{2})", param.Version, param.Branch, param.Sha);
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
  DotNetCoreRestore(param.Solution, new DotNetCoreRestoreSettings
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
  DotNetCoreBuild(param.Solution, new DotNetCoreBuildSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    ArgumentCustomization = args => args
      .Append($"/p:TreatWarningsAsErrors={param.IsReleaseBuild.ToString()}")
  });
});

Task("Tests")
  .Does(() =>
{
  foreach(var testProject in param.Projects.OnlyTests)
  {
    DotNetCoreTest(testProject.Path.FullPath, new DotNetCoreTestSettings
    {
      Configuration = param.Configuration,
      Verbosity = param.Verbosity,
      NoRestore = true,
      NoBuild = true,
      Logger = "trx",
      Filter = param.TestFilter,
      ResultsDirectory = param.Paths.Directories.TestResults,
      ArgumentCustomization = args => args
        .Append("/p:CollectCoverage=true")
        .Append("/p:CoverletOutputFormat=opencover")
        .Append($"/p:CoverletOutput=\"{MakeAbsolute(param.Paths.Directories.TestCoverage)}/\"")
    });
  }
});

Task("Sonar-Begin")
  .Does(() =>
{
  SonarBegin(new SonarBeginSettings
  {
    Url = param.SonarQubeCredentials.Url,
    Key = param.SonarQubeCredentials.Key,
    Login = param.SonarQubeCredentials.Token,
    Organization = param.SonarQubeCredentials.Organization,
    Branch = param.IsPullRequest ? null : param.Branch, // A pull request analysis can not have the branch analysis parameter 'sonar.branch.name'.
    Silent = true,
    Version = param.Version.Substring(5),
    PullRequestProvider = "GitHub",
    PullRequestGithubEndpoint = "https://api.github.com/",
    PullRequestGithubRepository = "HofmeisterAn/dotnet-testcontainers",
    PullRequestKey = System.Int32.TryParse(param.PullRequestId, out var id) ? id : (int?)null,
    PullRequestBranch = param.SourceBranch,
    PullRequestBase = param.TargetBranch,
    VsTestReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestResults)}/*.trx",
    OpenCoverReportsPath = $"{MakeAbsolute(param.Paths.Directories.TestCoverage)}/*.opencover.xml"
  });
});

Task("Sonar-End")
  .Does(() =>
{
  SonarEnd(new SonarEndSettings
  {
    Login = param.SonarQubeCredentials.Token
  });
});

Task("Create-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  DotNetCorePack(param.Projects.Testcontainers.Path.FullPath, new DotNetCorePackSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    NoBuild = true,
    IncludeSymbols = true,
    OutputDirectory = param.Paths.Directories.NugetRoot,
    ArgumentCustomization = args => args
      .Append($"/p:Version={param.Version}")
      .Append("/p:SymbolPackageFormat=snupkg")
  });
});

Task("Publish-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  foreach(var package in GetFiles($"{param.Paths.Directories.NugetRoot}/*.(nupkg|snupkgs)"))
  {
    DotNetCoreNuGetPush(package.FullPath, new DotNetCoreNuGetPushSettings
    {
      Source = param.NuGetCredentials.Source,
      ApiKey = param.NuGetCredentials.ApiKey
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
  .IsDependentOn("Publish-NuGet-Packages");

RunTarget(param.Target);
