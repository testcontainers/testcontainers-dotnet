namespace Testcontainers.Weaviate;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WeaviateContainer(WeaviateConfiguration configuration) : DockerContainer(configuration)
{
    /// <summary>
    /// Gets the Weaviate base address.
    /// </summary>
    /// <returns>The Weaviate base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WeaviateBuilder.WeaviateHttpPort)).ToString();
    }
}