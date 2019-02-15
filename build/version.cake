internal class BuildVersion
{
  private BuildVersion()
  {
  }

  public string Branch { get; private set; }
  public string Version { get; private set; }

  public static Lazy<BuildVersion> Instance(ICakeContext context)
  {
    var branch = context.GitBranchCurrent(".").FriendlyName;

    var version = context.ParseAssemblyInfo("src/SolutionInfo.cs").AssemblyVersion;

    var prerelease = context.EnvironmentVariable("PRERELEASE");

    var metadata = context.EnvironmentVariable("METADATA");

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
