namespace Testcontainers.Elasticsearch;

/// <summary>
/// Provides the Elasticsearch connection string.
/// </summary>
internal sealed class ElasticsearchConnectionStringProvider : ContainerConnectionStringProvider<ElasticsearchContainer, ElasticsearchConfiguration>
{
    /// <inheritdoc />
    protected override string GetHostConnectionString()
    {
        return Container.GetConnectionString();
    }
}