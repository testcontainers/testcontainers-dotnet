namespace Testcontainers.Milvus;

/// <summary>
/// Provides the Milvus connection string.
/// </summary>
internal sealed class MilvusConnectionStringProvider : ContainerConnectionStringProvider<MilvusContainer, MilvusConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetEndpoint().ToString();
    }
}