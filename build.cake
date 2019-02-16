#tool nuget:?package=MSBuild.SonarQube.Runner.Tool&version=4.3.1

#addin nuget:?package=Cake.Sonar&version=1.1.18

#addin nuget:?package=Cake.Git&version=0.19.0

#load "./build/parameters.cake"

BuildParameters param = BuildParameters.Instance(Context, "DotNet.Testcontainers").Value;

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

  Information("Building version {0} of .NET Testcontainers ({1})", param.Version, param.Branch);
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

Task("Build-Informations")
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
    NoRestore = true
  });
});

Task("Test")
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
      ResultsDirectory = param.Paths.Directories.TestResults,
      ArgumentCustomization = args => args
        .Append("/p:CollectCoverage=true")
        .Append("/p:CoverletOutputFormat=opencover")
        .Append($"/p:CoverletOutput=\"{MakeAbsolute(param.Paths.Directories.TestCoverage)}/\"")
    });
  }
});

Task("Copy-Artifacts")
  .Does(() =>
{
  var project = param.Projects.NoneTests.Single(p => "DotNet.Testcontainers".Equals(p.Name)).Path.FullPath;
  DotNetCorePublish(project, new DotNetCorePublishSettings
  {
    Configuration = param.Configuration,
    Verbosity = param.Verbosity,
    NoRestore = true,
    Framework = "netstandard2.0",
    OutputDirectory = param.Paths.Directories.ArtifactsBinStandard
  });

  CopyFileToDirectory("./LICENSE", param.Paths.Directories.ArtifactsBinStandard);
});

Task("SonarBegin")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  SonarBegin(new SonarBeginSettings
  {
    Url = param.SonarQubeCredentials.Url,
    Key = param.SonarQubeCredentials.Key,
    Login = param.SonarQubeCredentials.Token,
    Organization = param.SonarQubeCredentials.Organization,
    Branch = param.Branch,
    Silent = true,
    VsTestReportsPath = $"{param.Paths.Directories.TestResults}/*.trx",
    OpenCoverReportsPath = $"{param.Paths.Directories.TestCoverage}/coverage.opencover.xml"
  });
});

Task("SonarEnd")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  SonarEnd(new SonarEndSettings
  {
    Login = param.SonarQubeCredentials.Token
  });
});

Task("Create-NuGet-Packages")
  .Does(() =>
{
  var standardFullArtifactPath = MakeAbsolute(param.Paths.Directories.ArtifactsBinStandard).FullPath;
  var standardFullArtifactPathLength = standardFullArtifactPath.Length + 1;

  NuGetPack("./nuspec/DotNet.Testcontainers.symbols.nuspec", new NuGetPackSettings
  {
    Verbosity = NuGetVerbosity.Quiet,
    Version = param.Version,
    BasePath = param.Paths.Directories.ArtifactsBinStandard,
    OutputDirectory = param.Paths.Directories.NugetRoot,
    Symbols = true,
    NoPackageAnalysis = true
  });

  NuGetPack("./nuspec/DotNet.Testcontainers.nuspec", new NuGetPackSettings
  {
    Verbosity = NuGetVerbosity.Quiet,
    Version = param.Version,
    BasePath = param.Paths.Directories.ArtifactsBinStandard,
    OutputDirectory = param.Paths.Directories.NugetRoot,
    Symbols = false,
    NoPackageAnalysis = true,
    Files = GetFiles(param.Paths.Directories.ArtifactsBinStandard + "/**/*")
      .Select(file => file.FullPath.Substring(standardFullArtifactPathLength))
      .Select(file => new NuSpecContent { Source = file, Target = file })
      .ToArray()
  });
});

Task("Publish-NuGet-Packages")
  .WithCriteria(() => param.ShouldPublish)
  .Does(() =>
{
  foreach(var package in GetFiles($"{param.Paths.Directories.NugetRoot}/*.nupkg"))
  {
    NuGetPush(package, new NuGetPushSettings
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
  .IsDependentOn("Test");

Task("Sonar")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore-NuGet-Packages")
  .IsDependentOn("SonarBegin")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("SonarEnd");

Task("Publish")
  .IsDependentOn("Copy-Artifacts")
  .IsDependentOn("Create-NuGet-Packages")
  .IsDependentOn("Publish-NuGet-Packages");

RunTarget(param.Target);
