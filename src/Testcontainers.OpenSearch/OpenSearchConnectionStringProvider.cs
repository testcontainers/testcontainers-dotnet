namespace Testcontainers.OpenSearch;

/// <summary>
/// Provides the OpenSearch connection string.
/// </summary>
internal sealed class OpenSearchConnectionStringProvider : ContainerConnectionStringProvider<OpenSearchContainer, OpenSearchConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}