internal class BuildVersion
{
  private BuildVersion()
  {
  }

  public string Branch { get; private set; }
  public string Version { get; private set; }

  public static BuildVersion Instance(ICakeContext context)
  {
    var branch = context.EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? context.GitBranchCurrent(".").FriendlyName; // Azure Pipelines variable

    var buildNumber = context.EnvironmentVariable("BUILD_BUILDNUMBER"); // Azure Pipelines variable

    var metadata = context.EnvironmentVariable("METADATA");

    var version = context.ParseAssemblyInfo("src/SolutionInfo.cs").AssemblyVersion;

    if (!"master".Equals(branch))
    {
      version = $"{version}-beta";
    }

    if (!"master".Equals(branch) && !string.IsNullOrEmpty(buildNumber))
    {
      version = $"{version}.{buildNumber}";
    }

    if (!"master".Equals(branch) && !string.IsNullOrEmpty(metadata))
    {
      version = $"{version}+{metadata}";
    }

    return new BuildVersion
    {
      Branch = branch,
      Version = version
    };
  }
}
