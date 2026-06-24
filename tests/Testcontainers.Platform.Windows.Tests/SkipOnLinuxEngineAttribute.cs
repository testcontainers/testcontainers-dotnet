namespace Testcontainers.Tests;

[PublicAPI]
public sealed class SkipOnLinuxEngineAttribute : FactAttribute
{
    private static readonly bool IsLinuxEngineEnabled = DockerCli.PlatformIsEnabled(DockerCli.DockerPlatform.Linux);

    public SkipOnLinuxEngineAttribute(
        [CallerFilePath] [CanBeNull] string sourceFilePath = null,
        [CallerLineNumber] int sourceLineNumber = -1)
        : base(sourceFilePath, sourceLineNumber)
    {
        if (IsLinuxEngineEnabled)
        {
            Skip = "Docker Windows engine is not available.";
        }
    }
}