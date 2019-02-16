internal class BuildVersion
{
  private BuildVersion()
  {
  }

  public string Branch { get; private set; }
  public string Version { get; private set; }

  public static Lazy<BuildVersion> Instance(ICakeContext context)
  {
    var branch = context.EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? context.GitBranchCurrent(".").FriendlyName;

    var prerelease = context.EnvironmentVariable("PRERELEASE");

    var metadata = context.EnvironmentVariable("METADATA");

    var version = context.ParseAssemblyInfo("src/SolutionInfo.cs").AssemblyVersion;

    if (!"master".Equals(branch))
    {
      version = $"{version}-beta";
    }

    if (!"master".Equals(branch) && !string.IsNullOrEmpty(prerelease))
    {
      version = $"{version}.{prerelease}";
    }

    if (!"master".Equals(branch) && !string.IsNullOrEmpty(metadata))
    {
      version = $"{version}+{metadata}";
    }

    return new Lazy<BuildVersion>(() =>
    {
      return new BuildVersion
      {
        Branch = branch,
        Version = version
      };
    });
  }
}
