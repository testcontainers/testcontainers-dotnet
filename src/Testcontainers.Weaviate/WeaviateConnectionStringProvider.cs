namespace Testcontainers.Weaviate;

/// <summary>
/// Provides the Weaviate connection string.
/// </summary>
internal sealed class WeaviateConnectionStringProvider : ContainerConnectionStringProvider<WeaviateContainer, WeaviateConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetBaseAddress();
    }
}