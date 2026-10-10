namespace Testcontainers.Chroma;

/// <summary>
/// Provides the Chroma connection string.
/// </summary>
internal sealed class ChromaConnectionStringProvider : ContainerConnectionStringProvider<ChromaContainer, ChromaConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}
