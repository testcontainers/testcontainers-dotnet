namespace DotNet.Testcontainers.Tests
{
  using DotNet.Testcontainers.Commons;
  using Xunit;

  public sealed class SkipOnLinuxEngineAttribute : FactAttribute
  {
    private static readonly bool IsLinuxEngineEnabled = DockerCli.PlatformIsEnabled(DockerCli.DockerPlatform.Linux);

    public SkipOnLinuxEngineAttribute()
    {
      if (IsLinuxEngineEnabled)
      {
        this.Skip = "Docker Windows engine is not available.";
      }
    }
  }
}
