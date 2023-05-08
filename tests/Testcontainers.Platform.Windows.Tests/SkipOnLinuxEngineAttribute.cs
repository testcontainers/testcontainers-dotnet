namespace Testcontainers.Tests;

[PublicAPI]
public sealed class SkipOnLinuxEngineAttribute : FactAttribute
{
    private static readonly bool IsLinuxEngineEnabled = DockerCli.PlatformIsEnabled(DockerCli.DockerPlatform.Linux);

    public SkipOnLinuxEngineAttribute()
    {
        if (IsLinuxEngineEnabled)
        {
            Skip = "Docker Windows engine is not available.";
        }
    }
}