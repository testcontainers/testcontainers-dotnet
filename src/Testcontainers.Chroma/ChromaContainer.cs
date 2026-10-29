namespace Testcontainers.Chroma;

/// <inheritdoc cref="DockerContainer" />
[PublicAPI]
public sealed class ChromaContainer : DockerContainer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChromaContainer" /> class.
    /// </summary>
    /// <param name="configuration">The container configuration.</param>
    public ChromaContainer(ChromaConfiguration configuration)
        : base(configuration)
    {
    }

    /// <summary>
    /// Gets the Chroma base address.
    /// </summary>
    /// <returns>The Chroma base address.</returns>
    public string GetBaseAddress()
    {
        return new UriBuilder(Uri.UriSchemeHttp, Hostname, GetMappedPublicPort(ChromaBuilder.ChromaPort)).ToString();
    }
}