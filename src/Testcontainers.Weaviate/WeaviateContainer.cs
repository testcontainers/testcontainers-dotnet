namespace Testcontainers.Weaviate;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class WeaviateContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WeaviateContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public WeaviateContainer(WeaviateConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Weaviate base address.
    /// </summary>
    /// <returns>The Weaviate base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(WeaviateBuilder.WeaviateHttpPort)).ToString();
    }
}