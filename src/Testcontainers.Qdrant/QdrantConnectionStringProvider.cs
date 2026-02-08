namespace Testcontainers.Qdrant;

/// <summary>
/// Provides the Qdrant connection string.
/// </summary>
internal sealed class QdrantConnectionStringProvider : ContainerConnectionStringProvider<QdrantContainer, QdrantConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetHttpConnectionString();
    }
}