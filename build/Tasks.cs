namespace Testcontainers.Build;

public sealed class BuildContext(ICakeContext context) : FrostingContext(context)
{
    internal BuildParameters Parameters { get; } = BuildParameters.Instance(context);

    public void DotNetTest(SolutionProject project)
    {
        this.DotNetTest(project.Path.FullPath, new DotNetTestSettings
        {
            Configuration = Parameters.Configuration,
            Verbosity = Parameters.Verbosity,
            NoRestore = true,
            NoBuild = true,
            Collectors = ["XPlat Code Coverage;Format=opencover"],
            Filter = Parameters.TestFilter,
            ResultsDirectory = Parameters.Paths.Directories.TestResultsDirectoryPath,
            ArgumentCustomization = args => args
                // The windows-2025 GH-hosted runner no longer has cached images. Pulling the
                // servercore:ltsc2025 image takes significantly longer.
                .AppendSwitchQuoted("--blame-hang-timeout", "10m"),
        });
    }
}

public sealed class BuildLifetime : FrostingLifetime<BuildContext>
{
    public override void Setup(BuildContext context, ISetupContext info)
    {
        var param = context.Parameters;
        context.Information("Building version {0} of Testcontainers ({1}@{2})", param.Version, param.Branch, param.Sha);
    }

    public override void Teardown(BuildContext context, ITeardownContext info)
    {
    }
}

[TaskName("Clean")]
public sealed class CleanTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var deleteDirectorySettings = new DeleteDirectorySettings
        {
            Recursive = true,
            Force = true,
        };

        foreach (var directory in context.Parameters.Paths.Directories.ToClean)
        {
            if (context.DirectoryExists(directory))
            {
                context.DeleteDirectory(directory, deleteDirectorySettings);
            }
        }
    }
}

[TaskName("Restore-NuGet-Packages")]
public sealed class RestoreNuGetPackagesTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        context.DotNetRestore(param.Solution, new DotNetRestoreSettings
        {
            Verbosity = param.Verbosity,
        });
    }
}

[TaskName("Build-Information")]
public sealed class BuildInformationTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        foreach (var project in param.Projects.All)
        {
            Console.WriteLine($"{project.Name} [{project.Path.GetDirectory()}]");
        }
    }
}

[TaskName("Build")]
public sealed class BuildTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;

        // If a test project is specified, just build the project and its dependent projects to
        // save build time.
        var solutionOrProjectFilePath = string.IsNullOrEmpty(param.TestProject)
            ? param.Solution
            : param.Projects.OnlyTests.Single(testProject => testProject.Path.FullPath.EndsWith(param.TestProject + ".Tests.csproj")).Path.FullPath;

        context.DotNetBuild(solutionOrProjectFilePath, new DotNetBuildSettings
        {
            Configuration = param.Configuration,
            Verbosity = param.Verbosity,
            NoRestore = true,
            ArgumentCustomization = args => args
                .Append("/p:ContinuousIntegrationBuild=true"),
        });
    }
}

[TaskName("Test")]
public sealed class TestTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        var testProject = param.Projects.OnlyTests.Single(testProject => testProject.Path.FullPath.EndsWith(param.TestProject + ".Tests.csproj"));
        context.DotNetTest(testProject);
    }
}

[TaskName("Tests")]
public sealed class TestsTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        foreach (var testProject in param.Projects.OnlyTests)
        {
            context.DotNetTest(testProject);
        }
    }
}

[TaskName("Sonar-Begin")]
public sealed class SonarBeginTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        context.SonarBegin(new SonarBeginSettings
        {
            Url = param.SonarQubeCredentials.Url,
            Key = param.SonarQubeCredentials.Key,
            Token = param.SonarQubeCredentials.Token,
            Organization = param.SonarQubeCredentials.Organization,
            // A pull request analysis cannot have the branch analysis parameter 'sonar.branch.name'.
            Branch = param.IsPullRequest ? null : param.Branch,
            UseCoreClr = true,
            Silent = true,
            Version = param.Version[..5],
            PullRequestProvider = "GitHub",
            PullRequestGithubEndpoint = "https://api.github.com/",
            PullRequestGithubRepository = "testcontainers/testcontainers-dotnet",
            PullRequestKey = param.IsPullRequest && int.TryParse(param.PullRequestId, out var id) ? id : null,
            PullRequestBranch = param.SourceBranch,
            PullRequestBase = param.TargetBranch,
            OpenCoverReportsPath = $"{context.MakeAbsolute(param.Paths.Directories.TestResultsDirectoryPath)}/**/*.opencover.xml",
            VsTestReportsPath = $"{context.MakeAbsolute(param.Paths.Directories.TestResultsDirectoryPath)}/**/*.trx",
            ArgumentCustomization = args => args
                .Append("/d:sonar.scanner.scanAll=\"false\"")
                .Append("/d:sonar.scanner.skipJreProvisioning=\"true\""),
        });
    }
}

[TaskName("Sonar-End")]
public sealed class SonarEndTask : FrostingTask<BuildContext>
{
    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        context.SonarEnd(new SonarEndSettings
        {
            Token = param.SonarQubeCredentials.Token,
            UseCoreClr = true,
        });
    }
}

[TaskName("Create-NuGet-Packages")]
public sealed class CreateNuGetPackagesTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.Parameters.ShouldPublish;

    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        context.DotNetPack(param.Solution, new DotNetPackSettings
        {
            Configuration = param.Configuration,
            Verbosity = param.Verbosity,
            NoRestore = true,
            NoBuild = true,
            OutputDirectory = param.Paths.Directories.NuGetDirectoryPath,
            ArgumentCustomization = args => args
                .Append($"/p:Version={param.Version}"),
        });
    }
}

[TaskName("Sign-NuGet-Packages")]
public sealed class SignNuGetPackagesTask : FrostingTask<BuildContext>
{
    // We do not have access to a valid code signing certificate anymore.
    public override bool ShouldRun(BuildContext context) => /* context.Parameters.ShouldPublish */ false;

    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        context.StartProcess("dotnet", new ProcessSettings
        {
            Arguments = new ProcessArgumentBuilder()
                .Append("nuget")
                .Append("sign")
                .AppendSwitchQuoted("--certificate-path", param.Paths.Files.CodeSigningCertificateFilePath.FullPath)
                .AppendSwitchQuoted("--certificate-password", param.CodeSigningCertificateCredentials.Password)
                .AppendSwitchQuoted("--timestamper", "http://ts.quovadisglobal.com/eu")
                .Append($"{context.MakeAbsolute(param.Paths.Directories.NuGetDirectoryPath)}/**/*.nupkg"),
        });
    }
}

[TaskName("Publish-NuGet-Packages")]
public sealed class PublishNuGetPackagesTask : FrostingTask<BuildContext>
{
    public override bool ShouldRun(BuildContext context) => context.Parameters.ShouldPublish;

    public override void Run(BuildContext context)
    {
        var param = context.Parameters;
        foreach (var package in context.GetFiles($"{param.Paths.Directories.NuGetDirectoryPath}/*.nupkg"))
        {
            context.DotNetNuGetPush(package.FullPath, new DotNetNuGetPushSettings
            {
                Source = param.NuGetCredentials.Source,
                ApiKey = param.NuGetCredentials.ApiKey,
                SkipDuplicate = true,
            });
        }
    }
}

[TaskName("Default")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(RestoreNuGetPackagesTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestsTask))]
public sealed class DefaultTask : FrostingTask;

[TaskName("Sonar")]
[IsDependentOn(typeof(CleanTask))]
[IsDependentOn(typeof(RestoreNuGetPackagesTask))]
[IsDependentOn(typeof(SonarBeginTask))]
[IsDependentOn(typeof(BuildTask))]
[IsDependentOn(typeof(TestsTask))]
[IsDependentOn(typeof(SonarEndTask))]
public sealed class SonarTask : FrostingTask;

[TaskName("Publish")]
[IsDependentOn(typeof(CreateNuGetPackagesTask))]
[IsDependentOn(typeof(SignNuGetPackagesTask))]
[IsDependentOn(typeof(PublishNuGetPackagesTask))]
public sealed class PublishTask : FrostingTask;