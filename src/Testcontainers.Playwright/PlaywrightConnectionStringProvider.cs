namespace Testcontainers.Playwright;

/// <summary>
/// Provides the Playwright connection string.
/// </summary>
internal sealed class PlaywrightConnectionStringProvider : ContainerConnectionStringProvider<PlaywrightContainer, PlaywrightConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}