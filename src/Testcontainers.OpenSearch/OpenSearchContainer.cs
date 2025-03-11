namespace Testcontainers.OpenSearch;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class OpenSearchContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OpenSearchContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public OpenSearchContainer(OpenSearchConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Returns the url which is used to connect to OpenSearch cluster.
    /// </summary>
    /// <returns></returns>
    public string GetConnection()
    {
        return new UriBuilder(
            Uri.UriSchemeHttps,
            Hostname,
            GetMappedPublicPort(OpenSearchBuilder.OpenSearchApiPort)
        ).ToString();
    }
}